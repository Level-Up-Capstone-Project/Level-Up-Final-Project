using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Assessments;
using CapstoneProjectBE.ExternalServices.Assessments;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates Assessment service application behavior.
    /// </summary>
    public class AssessmentService : IAssessmentService
    {
        private readonly AppDbContext _context;
        private readonly N8nAssessmentQuestionGenerator _questionGenerator;

        public AssessmentService(
            AppDbContext context,
            N8nAssessmentQuestionGenerator questionGenerator)
        {
            _context = context;
            _questionGenerator = questionGenerator;
        }

        public async Task<AssessmentDto> StartAsync(
            Guid userId,
            StartAssessmentDto request,
            CancellationToken cancellationToken)
        {
            Guid skillId = request.SkillID!.Value;
            Guid roadmapItemId = request.RoadmapItemID!.Value;

            var assessmentContext = await _context.RoadmapItems
                .AsNoTracking()
                .Where(item =>
                    item.RoadmapItemID == roadmapItemId &&
                    item.SkillID == skillId &&
                    item.Roadmap.UserID == userId)
                .Select(item => new
                {
                    item.Skill.SkillID,
                    item.Skill.SkillName,
                    item.Skill.Category,
                    item.Skill.Description,
                    Course = item.Resources
                        .Where(resource => resource.IsCompleted)
                        .OrderByDescending(resource => resource.CompletedAt)
                        .ThenBy(resource => resource.LearningResource.ResourceName)
                        .Select(resource => new
                        {
                            resource.LearningResource.ResourceID,
                            resource.LearningResource.ResourceName,
                            resource.LearningResource.Provider,
                            resource.LearningResource.ProviderResourceID,
                            resource.LearningResource.ResourceType,
                            resource.LearningResource.Description
                        })
                        .FirstOrDefault()
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (assessmentContext is null)
            {
                throw new KeyNotFoundException(
                    "The roadmap item and skill could not be found for the current user.");
            }

            if (assessmentContext.Course is null)
            {
                throw new InvalidOperationException(
                    "Complete a recommended course before starting its knowledge check.");
            }

            var course = new N8nKnowledgeCheckCourse(
                assessmentContext.Course.ResourceID,
                assessmentContext.Course.ResourceName,
                assessmentContext.Course.Provider,
                assessmentContext.Course.ProviderResourceID,
                assessmentContext.Course.ResourceType,
                assessmentContext.Course.Description);

            int nextAttempt = (await _context.SkillAssessments
                .AsNoTracking()
                .Where(assessment =>
                    assessment.UserID == userId &&
                    assessment.SkillID == skillId)
                .MaxAsync(assessment => (int?)assessment.AttemptNumber, cancellationToken) ?? 0) + 1;

            // Never hold a database transaction or advisory lock while waiting for n8n.
            // If another request claims the predicted attempt, regenerate against the new attempt number.
            for (var allocationAttempt = 0; allocationAttempt < 3; allocationAttempt++)
            {
                IReadOnlyList<AssessmentQuestionSeed> generatedQuestions = await _questionGenerator.GenerateAsync(
                    skillId, assessmentContext.SkillName, course, nextAttempt, cancellationToken);

                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                if (_context.Database.IsNpgsql())
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"SELECT pg_advisory_xact_lock(hashtext({userId.ToString()}), hashtext({skillId.ToString()}))",
                        cancellationToken);
                }

                int availableAttempt = (await _context.SkillAssessments
                    .Where(assessment => assessment.UserID == userId && assessment.SkillID == skillId)
                    .MaxAsync(assessment => (int?)assessment.AttemptNumber, cancellationToken) ?? 0) + 1;
                if (availableAttempt != nextAttempt)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    nextAttempt = availableAttempt;
                    continue;
                }

                var assessment = new SkillAssessment
                {
                    SkillAssessmentID = Guid.NewGuid(), UserID = userId, SkillID = skillId,
                    RoadmapItemID = roadmapItemId, AttemptNumber = nextAttempt, PerfectScore = false,
                    GeneratedAt = DateTime.UtcNow,
                    Questions = generatedQuestions.Select((question, index) => new AssessmentQuestion
                    {
                        QuestionID = Guid.NewGuid(), QuestionText = question.QuestionText,
                        Choices = question.Choices.ToArray(), CorrectAnswer = question.CorrectAnswer,
                        QuestionOrder = index + 1
                    }).ToList()
                };
                _context.SkillAssessments.Add(assessment);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return MapAssessment(assessment);
            }

            throw new InvalidOperationException("Another assessment was started at the same time. Please try again.");
        }

        public async Task<AssessmentDto?> GetByIdAsync(
            Guid userId,
            Guid assessmentId,
            CancellationToken cancellationToken)
        {
            return await _context.SkillAssessments
                .AsNoTracking()
                .Where(assessment =>
                    assessment.SkillAssessmentID == assessmentId &&
                    assessment.UserID == userId &&
                    assessment.SkillID.HasValue &&
                    assessment.RoadmapItemID.HasValue &&
                    assessment.AttemptNumber.HasValue &&
                    assessment.GeneratedAt.HasValue)
                .Select(assessment => new AssessmentDto
                {
                    SkillAssessmentID = assessment.SkillAssessmentID,
                    SkillID = assessment.SkillID!.Value,
                    RoadmapItemID = assessment.RoadmapItemID!.Value,
                    AttemptNumber = assessment.AttemptNumber!.Value,
                    GeneratedAt = assessment.GeneratedAt!.Value,
                    Questions = assessment.Questions
                        .OrderBy(question => question.QuestionOrder)
                        .Select(question => new AssessmentQuestionDto
                        {
                            QuestionID = question.QuestionID,
                            QuestionText = question.QuestionText,
                            Choices = question.Choices,
                            QuestionOrder = question.QuestionOrder
                        })
                        .ToList()
                })
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<AssessmentResultDto?> SubmitAsync(
            Guid userId,
            Guid assessmentId,
            SubmitAssessmentDto request,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(cancellationToken);

            if (_context.Database.IsNpgsql())
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"SELECT pg_advisory_xact_lock(hashtext({userId.ToString()}), hashtext({assessmentId.ToString()}))",
                    cancellationToken);
            }

            SkillAssessment? assessment = await _context.SkillAssessments
                .Include(item => item.Skill)
                .Include(item => item.Questions)
                .SingleOrDefaultAsync(item =>
                    item.SkillAssessmentID == assessmentId &&
                    item.UserID == userId,
                    cancellationToken);

            if (assessment is null)
            {
                return null;
            }

            if (assessment.CompletedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "This assessment attempt has already been submitted.");
            }

            if (!assessment.SkillID.HasValue ||
                !assessment.AttemptNumber.HasValue ||
                assessment.Skill is null)
            {
                throw new InvalidOperationException(
                    "The assessment record is incomplete.");
            }

            Dictionary<Guid, AssessmentQuestion> questions = assessment.Questions
                .ToDictionary(question => question.QuestionID);

            bool hasDuplicateQuestionIds = request.Responses
                .Where(response => response.QuestionID.HasValue)
                .GroupBy(response => response.QuestionID!.Value)
                .Any(group => group.Count() > 1);

            if (hasDuplicateQuestionIds ||
                request.Responses.Count != questions.Count ||
                request.Responses.Any(response =>
                    !response.QuestionID.HasValue ||
                    !questions.ContainsKey(response.QuestionID.Value)))
            {
                throw new ArgumentException(
                    "Submit exactly one response for every assessment question.");
            }

            var canonicalAnswers = request.Responses.ToDictionary(
                response => response.QuestionID!.Value,
                response => questions[response.QuestionID!.Value].Choices.SingleOrDefault(choice =>
                    string.Equals(choice, response.UserAnswer.Trim(), StringComparison.OrdinalIgnoreCase)));
            if (canonicalAnswers.Values.Any(answer => answer is null))
            {
                throw new ArgumentException(
                    "Each response must be one of the choices supplied for its question.");
            }

            int correctAnswers = 0;

            foreach (AssessmentResponseRequestDto response in request.Responses)
            {
                AssessmentQuestion question = questions[response.QuestionID!.Value];
                string userAnswer = canonicalAnswers[question.QuestionID]!;
                bool isCorrect = string.Equals(
                    userAnswer,
                    question.CorrectAnswer.Trim(),
                    StringComparison.OrdinalIgnoreCase);

                if (isCorrect)
                {
                    correctAnswers++;
                }

                _context.AssessmentResponses.Add(new AssessmentResponse
                {
                    ResponseID = Guid.NewGuid(),
                    QuestionID = question.QuestionID,
                    UserAnswer = userAnswer,
                    IsCorrect = isCorrect
                });
            }

            decimal score = Math.Round(
                correctAnswers * 100m / questions.Count,
                2,
                MidpointRounding.AwayFromZero);

            DateTime completedAt = DateTime.UtcNow;
            bool perfectScore = score == 100m;

            assessment.ScorePercentage = score;
            assessment.PerfectScore = perfectScore;
            assessment.CompletedAt = completedAt;

            Guid skillId = assessment.SkillID.Value;
            UserSkill? userSkill = await _context.UserSkills
                .SingleOrDefaultAsync(item =>
                    item.UserID == userId && item.SkillID == skillId,
                    cancellationToken);

            if (userSkill is null)
            {
                userSkill = new UserSkill
                {
                    UserSkillID = Guid.NewGuid(),
                    UserID = userId,
                    SkillID = skillId,
                    IsSelfReported = false,
                    IsCVDetected = false,
                    IsKnowledgeVerified = false
                };

                _context.UserSkills.Add(userSkill);
            }

            if (perfectScore)
            {
                userSkill.IsKnowledgeVerified = true;
            }

            userSkill.Status = UserSkillStatusResolver.Resolve(
                userSkill.IsSelfReported,
                userSkill.IsCVDetected,
                userSkill.IsKnowledgeVerified,
                hasAssessmentAttempt: true);
            userSkill.UpdatedAt = completedAt;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new AssessmentResultDto
            {
                SkillAssessmentID = assessment.SkillAssessmentID,
                AttemptNumber = assessment.AttemptNumber.Value,
                ScorePercentage = score,
                CorrectAnswers = correctAnswers,
                TotalQuestions = questions.Count,
                PerfectScore = perfectScore,
                CompletedAt = completedAt,
                Skill = new AssessmentSkillResultDto
                {
                    SkillID = skillId,
                    SkillName = assessment.Skill.SkillName,
                    Status = UserSkillStatusResolver.ToApiStatus(userSkill.Status),
                    IsKnowledgeVerified = userSkill.IsKnowledgeVerified
                },
                Answers = assessment.Questions
                    .OrderBy(question => question.QuestionOrder)
                    .Select(question =>
                    {
                        string userAnswer = canonicalAnswers[question.QuestionID]!;
                        return new AssessmentAnswerReviewDto
                        {
                            QuestionID = question.QuestionID,
                            QuestionText = question.QuestionText,
                            UserAnswer = userAnswer,
                            CorrectAnswer = question.CorrectAnswer,
                            IsCorrect = string.Equals(userAnswer, question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase)
                        };
                    })
                    .ToList()
            };
        }

        public async Task<IReadOnlyList<AssessmentHistoryDto>> GetHistoryAsync(
            Guid userId,
            Guid skillId,
            CancellationToken cancellationToken)
        {
            return await _context.SkillAssessments
                .AsNoTracking()
                .Where(assessment =>
                    assessment.UserID == userId &&
                    assessment.SkillID == skillId &&
                    assessment.AttemptNumber.HasValue &&
                    assessment.GeneratedAt.HasValue)
                .OrderByDescending(assessment => assessment.AttemptNumber)
                .Select(assessment => new AssessmentHistoryDto
                {
                    SkillAssessmentID = assessment.SkillAssessmentID,
                    SkillID = assessment.SkillID!.Value,
                    RoadmapItemID = assessment.RoadmapItemID,
                    AttemptNumber = assessment.AttemptNumber!.Value,
                    ScorePercentage = assessment.ScorePercentage,
                    PerfectScore = assessment.PerfectScore,
                    GeneratedAt = assessment.GeneratedAt!.Value,
                    CompletedAt = assessment.CompletedAt
                })
                .ToListAsync(cancellationToken);
        }

        private static AssessmentDto MapAssessment(SkillAssessment assessment)
        {
            return new AssessmentDto
            {
                SkillAssessmentID = assessment.SkillAssessmentID,
                SkillID = assessment.SkillID!.Value,
                RoadmapItemID = assessment.RoadmapItemID!.Value,
                AttemptNumber = assessment.AttemptNumber!.Value,
                GeneratedAt = assessment.GeneratedAt!.Value,
                Questions = assessment.Questions
                    .OrderBy(question => question.QuestionOrder)
                    .Select(question => new AssessmentQuestionDto
                    {
                        QuestionID = question.QuestionID,
                        QuestionText = question.QuestionText,
                        Choices = question.Choices,
                        QuestionOrder = question.QuestionOrder
                    })
                    .ToList()
            };
        }
    }
}
