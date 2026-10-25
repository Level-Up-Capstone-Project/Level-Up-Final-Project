using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Skills;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates User Skill service application behavior.
    /// </summary>
    public class UserSkillService : IUserSkillService
    {
        private readonly AppDbContext _context;

        public UserSkillService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<UserSkillDto>> GetForUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var rows = await _context.UserSkills.AsNoTracking().Where(userSkill => userSkill.UserID == userId).OrderBy(userSkill => userSkill.Skill.SkillName).Select(userSkill => new
            {
                userSkill.UserSkillID,
                userSkill.SkillID,
                userSkill.Skill.SkillName,
                userSkill.Skill.Category,
                userSkill.Status,
                userSkill.IsSelfReported,
                userSkill.IsCVDetected,
                userSkill.IsKnowledgeVerified,
                HasAssessmentAttempt = _context.SkillAssessments.Any(assessment =>
                    assessment.UserID == userId &&
                    assessment.SkillID == userSkill.SkillID &&
                    assessment.CompletedAt != null),
                userSkill.UpdatedAt
            }).ToListAsync(cancellationToken);

            return rows.Select(row => new UserSkillDto
            {
                UserSkillID = row.UserSkillID,
                SkillID = row.SkillID,
                SkillName = row.SkillName,
                Category = row.Category,
                Status = UserSkillStatusResolver.ToApiStatus(UserSkillStatusResolver.Resolve(
                    row.IsSelfReported,
                    row.IsCVDetected,
                    row.IsKnowledgeVerified,
                    row.HasAssessmentAttempt)),
                IsSelfReported = row.IsSelfReported,
                IsCVDetected = row.IsCVDetected,
                IsKnowledgeVerified = row.IsKnowledgeVerified,
                UpdatedAt = row.UpdatedAt
            }).ToList();
        }

        public async Task<UserSkillDto?> AddSelfReportedAsync(Guid userId, Guid skillId, CancellationToken cancellationToken)
        {
            if(skillId == Guid.Empty)
            {
                return null;
            }

            bool userExists = await _context.Users.AnyAsync(user => user.UserID == userId, cancellationToken);

            if(!userExists)
            {
                return null;
            }

            Skill? skill = await _context.Skills.AsNoTracking().SingleOrDefaultAsync(skill => skill.SkillID == skillId, cancellationToken);

            if (skill is null)
            {
                return null;
            }

            UserSkill? userSkill = await _context.UserSkills.SingleOrDefaultAsync(userSkill => userSkill.UserID == userId && userSkill.SkillID == skillId, cancellationToken);

            if (userSkill is null)
            {
                userSkill = new UserSkill
                {
                    UserSkillID = Guid.NewGuid(),
                    UserID = userId,
                    SkillID = skillId
                };

                _context.UserSkills.Add(userSkill);
            }

            userSkill.IsSelfReported = true;
            userSkill.Status = UserSkillStatusResolver.Resolve(userSkill.IsSelfReported, userSkill.IsCVDetected, userSkill.IsKnowledgeVerified);
            userSkill.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return MapToDto(userSkill, skill);
        }

        public async Task<bool> RemoveSelfReportedAsync(Guid userId, Guid skillId,  CancellationToken cancellationToken)
        {
            UserSkill? userSkill = await _context.UserSkills.SingleOrDefaultAsync(userSkill => userSkill.UserID == userId && userSkill.SkillID == skillId, cancellationToken);

            if(userSkill is null)
            {
                return false;
            }

            userSkill.IsSelfReported = false;

            bool hasAssessmentAttempt = await _context.SkillAssessments
                .AnyAsync(assessment =>
                    assessment.UserID == userId &&
                    assessment.SkillID == skillId &&
                    assessment.CompletedAt != null,
                    cancellationToken);

            if(!userSkill.IsCVDetected && !userSkill.IsKnowledgeVerified && !hasAssessmentAttempt)
            {
                _context.UserSkills.Remove(userSkill);
            }
            else
            {
                userSkill.Status = UserSkillStatusResolver.Resolve(
                    userSkill.IsSelfReported,
                    userSkill.IsCVDetected,
                    userSkill.IsKnowledgeVerified,
                    hasAssessmentAttempt);
                userSkill.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static UserSkillDto MapToDto(UserSkill userSkill, Skill skill)
        {
            return new UserSkillDto
            {
                UserSkillID = userSkill.UserSkillID,
                SkillID = userSkill.SkillID,
                SkillName = skill.SkillName,
                Category = skill.Category,
                Status = UserSkillStatusResolver.ToApiStatus(userSkill.Status),
                IsSelfReported = userSkill.IsSelfReported,
                IsCVDetected = userSkill.IsCVDetected,
                IsKnowledgeVerified = userSkill.IsKnowledgeVerified,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
