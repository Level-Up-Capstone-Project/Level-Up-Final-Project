using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.CVs;
using CapstoneProjectBE.ExternalServices.Storage;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates CV service application behavior.
    /// </summary>
    public class CVService : ICVService
    {
        private readonly AppDbContext _context;
        private readonly ICVStorageService _storageService;
        private readonly ILogger<CVService> _logger;

        public CVService(AppDbContext context, ICVStorageService storageService, ILogger<CVService> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<CVDto> UploadAsync(Guid userId, IFormFile file, CancellationToken cancellationToken)
        {
            bool userExists = await _context.Users.AsNoTracking().AnyAsync(user => user.UserID == userId, cancellationToken);

            if (!userExists)
            {
                throw new KeyNotFoundException("The user cannot be found.");
            }

            Guid cvId = Guid.NewGuid();

            string fileLocation = await _storageService.UploadAsync(userId, cvId, file, cancellationToken);

            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                await _context.CVs.Where(cv => cv.UserID == userId && cv.IsCurrent).ExecuteUpdateAsync(update => update.SetProperty(cv => cv.IsCurrent, false), cancellationToken);
                await ClearCVDetectedSkillsAsync(userId, cancellationToken);
                var cv = new CV
                {
                    CVID = cvId,
                    UserID = userId,
                    FileName = Path.GetFileName(file.FileName),
                    FileLocation = fileLocation,
                    FileType = file.ContentType,
                    IsCurrent = true,
                    UploadedAt = DateTime.UtcNow
                };

                _context.CVs.Add(cv);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return MapToDto(cv);
            }
            catch
            {
                try
                {
                    await _storageService.DeleteAsync(fileLocation, CancellationToken.None);
                }
                catch(Exception cleanupException)
                {
                    _logger.LogWarning(cleanupException, "Failed to remove orphaned CV file {FileLocation}.", fileLocation);
                }
                throw;
            }
        }

        public async Task<IReadOnlyList<CVDto>> GetForUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.CVs.AsNoTracking().Where(cv => cv.UserID == userId).OrderByDescending(cv => cv.UploadedAt).Select(cv => new CVDto
            {
                CVID = cv.CVID,
                FileName = cv.FileName,
                FileType = cv.FileType,
                IsCurrent = cv.IsCurrent,
                UploadedAt = cv.UploadedAt
            }).ToListAsync(cancellationToken);
        }

        public async Task<CVDto?> GetCurrentAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.CVs.AsNoTracking().Where(cv => cv.UserID == userId && cv.IsCurrent).Select(cv => new CVDto
            {
                CVID = cv.CVID,
                FileName = cv.FileName,
                FileType = cv.FileType,
                IsCurrent = cv.IsCurrent,
                UploadedAt = cv.UploadedAt
            }).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<CVDto?> GetByIdAsync(Guid userId, Guid cvId, CancellationToken cancellationToken)
        {
            return await _context.CVs.AsNoTracking().Where(cv => cv.UserID == userId && cv.CVID == cvId).Select(cv => new CVDto
            {
                CVID = cv.CVID,
                FileName = cv.FileName,
                FileType = cv.FileType,
                IsCurrent = cv.IsCurrent,
                UploadedAt = cv.UploadedAt
            }).SingleOrDefaultAsync(cancellationToken);
        }

        private static CVDto MapToDto(CV cv)
        {
            return new CVDto
            {
                CVID = cv.CVID,
                FileName = cv.FileName,
                FileType = cv.FileType,
                IsCurrent = cv.IsCurrent,
                UploadedAt = cv.UploadedAt
            };
        }

        private async Task ClearCVDetectedSkillsAsync(Guid userId, CancellationToken cancellationToken)
        {
            List<UserSkill> cvDetectedSkills = await _context.UserSkills.Where(userSkill => userSkill.UserID == userId && userSkill.IsCVDetected).ToListAsync(cancellationToken);

            List<Guid> assessmentAttemptSkillIdList = await _context.SkillAssessments
                .AsNoTracking()
                .Where(assessment =>
                    assessment.UserID == userId &&
                    assessment.SkillID.HasValue &&
                    assessment.CompletedAt != null)
                .Select(assessment => assessment.SkillID!.Value)
                .ToListAsync(cancellationToken);

            HashSet<Guid> assessmentAttemptSkillIds = assessmentAttemptSkillIdList.ToHashSet();

            foreach (UserSkill userSkill in cvDetectedSkills)
            {
                userSkill.IsCVDetected = false;

                bool hasAssessmentAttempt = assessmentAttemptSkillIds.Contains(userSkill.SkillID);

                if (!userSkill.IsSelfReported && !userSkill.IsKnowledgeVerified && !hasAssessmentAttempt)
                {
                    _context.UserSkills.Remove(userSkill);
                    continue;
                }

                userSkill.Status = UserSkillStatusResolver.Resolve(
                    userSkill.IsSelfReported,
                    false,
                    userSkill.IsKnowledgeVerified,
                    hasAssessmentAttempt);
                userSkill.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
