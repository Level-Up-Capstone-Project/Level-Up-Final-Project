using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.SkillGap;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    public class SkillGapService : ISkillGapService
    {
        private readonly AppDbContext _context;

        public SkillGapService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SkillGapDto?> GetSkillGapAsync(Guid userId, CancellationToken cancellationToken)
        {
            Guid? targetCareerId = await _context.UserProfiles.AsNoTracking().Where(profile => profile.UserID == userId).Select(profile => profile.TargetCareerID).SingleOrDefaultAsync(cancellationToken);
            
            if(!targetCareerId.HasValue)
            {
                return null;
            }

            var career = await _context.Careers.AsNoTracking().Where(career => career.CareerID == targetCareerId.Value).Select(career => new
            {
                career.CareerID,
                career.CareerName,
                RequiredSkills = career.CareerSkills.OrderByDescending(careerSkill => careerSkill.IsCoreSkill).ThenByDescending(careerSkill => careerSkill.ImportanceLevel).Select(careerSkill => new
                {
                    careerSkill.SkillID,
                    careerSkill.Skill.SkillName,
                    careerSkill.Skill.Category,
                    careerSkill.ImportanceLevel,
                    careerSkill.IsCoreSkill
                }).ToList()
            }).SingleOrDefaultAsync(cancellationToken);

            if(career is null)
            {
                return null;
            }

            var evidence = await _context.UserSkills.AsNoTracking().Where(UserSkill => UserSkill.UserID == userId).Select(userSkill => new
            {
                userSkill.SkillID,
                userSkill.IsSelfReported,
                userSkill.IsCVDetected,
                userSkill.IsKnowledgeVerified,
                HasAssessmentAttempt = _context.SkillAssessments.Any(assessment =>
                    assessment.UserID == userId &&
                    assessment.SkillID == userSkill.SkillID &&
                    assessment.CompletedAt != null)
            }).ToDictionaryAsync(userSkill => userSkill.SkillID, cancellationToken);

            decimal totalWeight = 0;
            decimal achievedWeight = 0;
            var skillItems = new List<SkillGapItemDto>();

            foreach(var requiredSkill in career.RequiredSkills)
            {
                string databaseStatus = UserSkillStatusResolver.Missing;

                if(evidence.TryGetValue(requiredSkill.SkillID, out var userSkill))
                {
                    databaseStatus = UserSkillStatusResolver.Resolve(
                        userSkill.IsSelfReported,
                        userSkill.IsCVDetected,
                        userSkill.IsKnowledgeVerified,
                        userSkill.HasAssessmentAttempt);
                }

                totalWeight += requiredSkill.ImportanceLevel;
                achievedWeight += requiredSkill.ImportanceLevel * UserSkillStatusResolver.ToMatchWeight(databaseStatus);

                skillItems.Add(new SkillGapItemDto
                {
                    SkillID = requiredSkill.SkillID,
                    SkillName = requiredSkill.SkillName,
                    Category = requiredSkill.Category,
                    ImportanceLevel = requiredSkill.ImportanceLevel,
                    IsCoreSkill = requiredSkill.IsCoreSkill,
                    Status = UserSkillStatusResolver.ToApiStatus(databaseStatus)
                });
            }

            decimal matchPercentage = totalWeight == 0 ? 0 : Math.Round(achievedWeight / totalWeight * 100, 2);

            return new SkillGapDto
            {
                Career = new SkillGapCareerDto
                {
                    CareerID = career.CareerID,
                    CareerName = career.CareerName
                },
                SkillMatchPercentage = matchPercentage,
                Skills = skillItems
            };
        }
    }
}
