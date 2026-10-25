using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.CVs;
using CapstoneProjectBE.ExternalServices.CVAnalysis;
using CapstoneProjectBE.ExternalServices.Storage;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates CVAnalysis service application behavior.
    /// </summary>
    public class CVAnalysisService : ICVAnalysisService
    {
        private readonly AppDbContext _context;
        private readonly ICVStorageService _cvStorageService;
        private readonly ICVAnalysisProvider _cvAnalysisProvider;

        public CVAnalysisService(AppDbContext appDbContext, ICVStorageService cvStorageService, ICVAnalysisProvider cvAnalysisProvider)
        {
            _context = appDbContext;
            _cvStorageService = cvStorageService;
            _cvAnalysisProvider = cvAnalysisProvider;
        }

        public async Task<CVAnalysisDto?> AnalyseAsync(Guid userId, Guid cvId, CancellationToken cancellationToken, Func<int, Task>? reportProgress = null)
        {
            CV? cv = await _context.CVs.AsNoTracking().SingleOrDefaultAsync(cv => cv.CVID == cvId && cv.UserID == userId, cancellationToken);

            if (cv is null)
            {
                return null;
            }

            _cvAnalysisProvider.EnsureConfigured();

            if (reportProgress is not null) await reportProgress(10);
            byte[] fileBytes = await _cvStorageService.DownloadAsync(cv.FileLocation, cancellationToken);

            if (reportProgress is not null) await reportProgress(25);
            if (reportProgress is not null) await reportProgress(60);
            List<CVSkillCandidate> knownSkills = await _context.Skills.AsNoTracking().OrderBy(skill => skill.SkillName).Select(skill => new CVSkillCandidate(skill.SkillID, skill.SkillName)).ToListAsync(cancellationToken);
            var target = await _context.Users.AsNoTracking().Where(user => user.UserID == userId)
                .Select(user => new
                {
                    TargetCareerID = user.UserProfile == null ? null : user.UserProfile.TargetCareerID,
                    CareerName = user.UserProfile == null || user.UserProfile.TargetCareer == null ? null : user.UserProfile.TargetCareer.CareerName,
                })
                .SingleOrDefaultAsync(cancellationToken);
            CVAnalysisResult result = await _cvAnalysisProvider.AnalyseAsync(new CVAnalysisRequest(Guid.NewGuid(), cv.CVID,
                target?.TargetCareerID, target?.CareerName, cv.FileName, cv.FileType, fileBytes, knownSkills), cancellationToken);
            if (reportProgress is not null) await reportProgress(80);
            Guid analysisId = Guid.NewGuid();
            DateTime analysedAt = DateTime.UtcNow;

            var analysis = new Models.CVAnalysis
            {
                AnalysisID = analysisId,
                CVID = cv.CVID,
                QualityScore = result.QualityScore,
                ATSScore = result.ATSScore,
                FormattingScore = result.FormattingScore,
                Strengths = result.Strengths,
                Weaknesses = result.Weaknesses,
                Recommendations = result.Recommendations,
                AnalysedAt = analysedAt
            };

            _context.CVAnalyses.Add(analysis);

            foreach(CVSkillCandidate skill in result.DetectedSkills)
            {
                _context.CVAnalysisSkills.Add(new CVAnalysisSkill
                {
                    CVAnalysisSkillID = Guid.NewGuid(),
                    AnalysisID = analysisId,
                    SkillID = skill.SkillID
                });
            }

            bool isCurrentCV = await _context.CVs.AnyAsync(currentCv => currentCv.CVID == cvId && currentCv.UserID == userId && currentCv.IsCurrent, cancellationToken);

            if(isCurrentCV)
            {
                await SynchroniseCVDetectedSkillsAsync(userId, result.DetectedSkills, cancellationToken);
            }

            if (reportProgress is not null) await reportProgress(90);
            await _context.SaveChangesAsync(cancellationToken);

            return new CVAnalysisDto
            {
                AnalysisID = analysisId,
                CVID = cv.CVID,
                QualityScore = result.QualityScore,
                ATSScore = result.ATSScore,
                FormattingScore = result.FormattingScore,
                Strengths = result.Strengths,
                Weaknesses = result.Weaknesses,
                Recommendations = result.Recommendations,
                AnalysedAt = analysedAt,
                DetectedSkills = result.DetectedSkills.Select(skill => new DetectedSkillDto
                    {
                        SkillID = skill.SkillID,
                        SkillName = skill.SkillName
                    }).ToList()
            };
        }

        public async Task<IReadOnlyList<CVAnalysisDto>?> GetForCVAsync(Guid userId, Guid cvId, CancellationToken cancellationToken)
        {
            bool cvExists = await _context.CVs.AsNoTracking().AnyAsync(cv => cv.CVID == cvId && cv.UserID ==  userId, cancellationToken);

            if (!cvExists)
            {
                return null;
            }

            return await _context.CVAnalyses.AsNoTracking().Where(analysis => analysis.CVID == cvId).OrderByDescending(analysis => analysis.AnalysedAt).Select(analysis => new CVAnalysisDto
            {
                AnalysisID = analysis.AnalysisID,
                CVID = analysis.CVID,
                QualityScore = analysis.QualityScore,
                ATSScore = analysis.ATSScore,
                FormattingScore = analysis.FormattingScore,
                Strengths = analysis.Strengths,
                Weaknesses = analysis.Weaknesses,
                Recommendations = analysis.Recommendations,
                AnalysedAt = analysis.AnalysedAt,
                DetectedSkills = analysis.DetectedSkills.OrderBy(link => link.Skill.SkillName).Select(link => new DetectedSkillDto
                        {
                            SkillID = link.SkillID,
                            SkillName = link.Skill.SkillName
                        }).ToList()
            }).ToListAsync(cancellationToken);
        }

        public async Task<CVAnalysisDto?> GetCurrentAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.CVAnalyses.AsNoTracking().Where(analysis => analysis.CV.UserID == userId && analysis.CV.IsCurrent).OrderByDescending(analysis => analysis.AnalysedAt).Select(analysis => new CVAnalysisDto
            {
                AnalysisID = analysis.AnalysisID,
                CVID = analysis.CVID,
                QualityScore = analysis.QualityScore,
                ATSScore = analysis.ATSScore,
                FormattingScore = analysis.FormattingScore,
                Strengths = analysis.Strengths,
                Weaknesses = analysis.Weaknesses,
                Recommendations = analysis.Recommendations,
                AnalysedAt = analysis.AnalysedAt,
                DetectedSkills = analysis.DetectedSkills.OrderBy(link => link.Skill.SkillName).Select(link => new DetectedSkillDto
                {
                    SkillID = link.SkillID,
                    SkillName = link.Skill.SkillName
                }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);
        }

        private async Task SynchroniseCVDetectedSkillsAsync(Guid userId, IReadOnlyList<CVSkillCandidate> detectedSkills, CancellationToken cancellationToken)
        {
            HashSet<Guid> detectedSkillIds = detectedSkills.Select(skill => skill.SkillID).ToHashSet();

            List<UserSkill> existingUserSkills = await _context.UserSkills.Where(userSkill => userSkill.UserID == userId).ToListAsync(cancellationToken);

            List<Guid> assessmentAttemptSkillIdList = await _context.SkillAssessments
                .AsNoTracking()
                .Where(assessment =>
                    assessment.UserID == userId &&
                    assessment.SkillID.HasValue &&
                    assessment.CompletedAt != null)
                .Select(assessment => assessment.SkillID!.Value)
                .ToListAsync(cancellationToken);

            HashSet<Guid> assessmentAttemptSkillIds = assessmentAttemptSkillIdList.ToHashSet();

            Dictionary<Guid, UserSkill> existingBySkillId = existingUserSkills.ToDictionary(userSkill => userSkill.SkillID);

            foreach (UserSkill userSkill in existingUserSkills)
            {
                userSkill.IsCVDetected = detectedSkillIds.Contains(userSkill.SkillID);

                bool hasAssessmentAttempt = assessmentAttemptSkillIds.Contains(userSkill.SkillID);

                if (!userSkill.IsSelfReported && !userSkill.IsCVDetected && !userSkill.IsKnowledgeVerified && !hasAssessmentAttempt)
                {
                    _context.UserSkills.Remove(userSkill);
                    continue;
                }

                userSkill.Status = UserSkillStatusResolver.Resolve(
                    userSkill.IsSelfReported,
                    userSkill.IsCVDetected,
                    userSkill.IsKnowledgeVerified,
                    hasAssessmentAttempt);
                userSkill.UpdatedAt = DateTime.UtcNow;
            }

            foreach (Guid skillId in detectedSkillIds)
            {
                if (existingBySkillId.ContainsKey(skillId))
                {
                    continue;
                }

                _context.UserSkills.Add(new UserSkill
                {
                    UserSkillID = Guid.NewGuid(),
                    UserID = userId,
                    SkillID = skillId,
                    IsSelfReported = false,
                    IsCVDetected = true,
                    IsKnowledgeVerified = false,
                    Status = UserSkillStatusResolver.Resolve(false, true, false),
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }   
    }
}
