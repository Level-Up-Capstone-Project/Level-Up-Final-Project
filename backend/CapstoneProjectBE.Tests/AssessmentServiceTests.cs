using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Assessments;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace CapstoneProjectBE.Tests;

/// <summary>
/// Verifies Assessment Service behavior.
/// </summary>
public sealed class AssessmentServiceTests
{
    [Fact]
    public async Task CandidateQuestionResponseOmitsAnswerKeysAndChecksOwnership()
    {
        await using var context = CreateContext();
        var seed = await SeedAssessmentAsync(context);
        var assessment = await context.SkillAssessments.SingleAsync();
        assessment.RoadmapItemID = Guid.NewGuid();
        await context.SaveChangesAsync();
        var service = new AssessmentService(context, null!);
        Assert.Null(await service.GetByIdAsync(Guid.NewGuid(), seed.AssessmentId, default));
        var result = await service.GetByIdAsync(seed.UserId, seed.AssessmentId, default);
        var json = System.Text.Json.JsonSerializer.Serialize(result);
        Assert.DoesNotContain("CorrectAnswer", json);
        Assert.DoesNotContain("MarkingGuidance", json);
        Assert.Contains("MultipleChoice", json);
    }

    [Fact]
    public async Task SubmitReturnsScoredContractAndOrderedAnswerReview()
    {
        await using var context = CreateContext();
        var seed = await SeedAssessmentAsync(context);
        var service = new AssessmentService(context, null!);

        AssessmentResultDto? result = await service.SubmitAsync(seed.UserId, seed.AssessmentId,
            Submission((seed.SecondQuestionId, "cat"), (seed.FirstQuestionId, "  alpha ")), default);

        Assert.NotNull(result);
        Assert.Equal(seed.AssessmentId, result.SkillAssessmentID);
        Assert.Equal(1, result.AttemptNumber);
        Assert.Equal(50m, result.ScorePercentage);
        Assert.Equal(1, result.CorrectAnswers);
        Assert.Equal(2, result.TotalQuestions);
        Assert.False(result.PerfectScore);
        Assert.Equal(seed.SkillId, result.Skill.SkillID);
        Assert.Equal("SQL", result.Skill.SkillName);
        Assert.Equal("Partial", result.Skill.Status);
        Assert.False(result.Skill.IsKnowledgeVerified);
        Assert.Equal([seed.FirstQuestionId, seed.SecondQuestionId], result.Answers.Select(answer => answer.QuestionID));
        Assert.Equal("Alpha", result.Answers[0].UserAnswer);
        Assert.True(result.Answers[0].IsCorrect);
        Assert.False(result.Answers[1].IsCorrect);
        Assert.Equal(2, await context.AssessmentResponses.CountAsync());
    }

    [Fact]
    public async Task PerfectSubmissionMarksSkillAsKnowledgeVerified()
    {
        await using var context = CreateContext();
        var seed = await SeedAssessmentAsync(context);
        var service = new AssessmentService(context, null!);

        AssessmentResultDto? result = await service.SubmitAsync(seed.UserId, seed.AssessmentId,
            Submission((seed.FirstQuestionId, "Alpha"), (seed.SecondQuestionId, "Dog")), default);

        Assert.NotNull(result);
        Assert.Equal(100m, result.ScorePercentage);
        Assert.True(result.PerfectScore);
        Assert.True(result.Skill.IsKnowledgeVerified);
        Assert.Equal("Have", result.Skill.Status);
        UserSkill skill = await context.UserSkills.SingleAsync();
        Assert.True(skill.IsKnowledgeVerified);
        Assert.Equal(UserSkillStatusResolver.Have, skill.Status);
    }

    [Fact]
    public async Task SubmitRejectsAnswerOutsideSuppliedChoicesWithoutSaving()
    {
        await using var context = CreateContext();
        var seed = await SeedAssessmentAsync(context);
        var service = new AssessmentService(context, null!);

        var error = await Assert.ThrowsAsync<ArgumentException>(() => service.SubmitAsync(
            seed.UserId, seed.AssessmentId,
            Submission((seed.FirstQuestionId, "Not a choice"), (seed.SecondQuestionId, "Dog")), default));

        Assert.Contains("one of the choices", error.Message);
        Assert.Empty(context.AssessmentResponses);
        Assert.Null((await context.SkillAssessments.SingleAsync()).CompletedAt);
    }

    [Fact]
    public async Task SubmitRejectsRepeatSubmission()
    {
        await using var context = CreateContext();
        var seed = await SeedAssessmentAsync(context);
        var service = new AssessmentService(context, null!);
        var request = Submission((seed.FirstQuestionId, "Alpha"), (seed.SecondQuestionId, "Dog"));
        await service.SubmitAsync(seed.UserId, seed.AssessmentId, request, default);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SubmitAsync(seed.UserId, seed.AssessmentId, request, default));

        Assert.Contains("already been submitted", error.Message);
        Assert.Equal(2, await context.AssessmentResponses.CountAsync());
    }

    [Fact]
    public async Task SubmitDoesNotExposeAnotherUsersAssessment()
    {
        await using var context = CreateContext();
        var seed = await SeedAssessmentAsync(context);
        var service = new AssessmentService(context, null!);

        AssessmentResultDto? result = await service.SubmitAsync(Guid.NewGuid(), seed.AssessmentId,
            Submission((seed.FirstQuestionId, "Alpha"), (seed.SecondQuestionId, "Dog")), default);

        Assert.Null(result);
        Assert.Empty(context.AssessmentResponses);
    }

    private static AppDbContext CreateContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options);

    private static SubmitAssessmentDto Submission(params (Guid QuestionId, string Answer)[] responses) => new()
    {
        Responses = responses.Select(response => new AssessmentResponseRequestDto
        {
            QuestionID = response.QuestionId,
            UserAnswer = response.Answer
        }).ToList()
    };

    private static async Task<AssessmentSeed> SeedAssessmentAsync(AppDbContext context)
    {
        var userId = Guid.NewGuid();
        var skillId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var firstQuestionId = Guid.NewGuid();
        var secondQuestionId = Guid.NewGuid();
        var user = new User
        {
            UserID = userId,
            Email = $"{userId:N}@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        var skill = new Skill
        {
            SkillID = skillId,
            SkillName = "SQL",
            Category = "Technical",
            Description = "Query language"
        };
        var assessment = new SkillAssessment
        {
            SkillAssessmentID = assessmentId,
            UserID = userId,
            User = user,
            SkillID = skillId,
            Skill = skill,
            AttemptNumber = 1,
            GeneratedAt = DateTime.UtcNow,
            Questions =
            [
                new AssessmentQuestion
                {
                    QuestionID = secondQuestionId,
                    QuestionText = "Choose the dog",
                    Choices = ["Cat", "Dog", "Bird"],
                    CorrectAnswer = "Dog",
                    QuestionOrder = 2
                },
                new AssessmentQuestion
                {
                    QuestionID = firstQuestionId,
                    QuestionText = "Choose alpha",
                    Choices = ["Alpha", "Beta", "Gamma"],
                    CorrectAnswer = "Alpha",
                    QuestionOrder = 1
                }
            ]
        };
        context.AddRange(user, skill, assessment);
        await context.SaveChangesAsync();
        return new(userId, skillId, assessmentId, firstQuestionId, secondQuestionId);
    }

    /// <summary>
    /// Carries Assessment Seed values used by test fixtures.
    /// </summary>
    private sealed record AssessmentSeed(
        Guid UserId,
        Guid SkillId,
        Guid AssessmentId,
        Guid FirstQuestionId,
        Guid SecondQuestionId);
}
