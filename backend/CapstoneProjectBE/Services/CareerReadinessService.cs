using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.CareerReadiness;
using CapstoneProjectBE.DTOs.SkillGap;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates Career Readiness service application behavior.
    /// </summary>
    public class CareerReadinessService : ICareerReadinessService
    {
        private readonly AppDbContext _context;
        private readonly ISkillGapService _skillGapService;

        public CareerReadinessService(AppDbContext context, ISkillGapService skillGapService)
        {
            _context = context;
            _skillGapService = skillGapService;
        }

        public async Task<CareerReadinessDto?>GetCareerReadinessAsync(Guid userId, CancellationToken cancellationToken)
        {
            SkillGapDto? skillGap = await _skillGapService.GetSkillGapAsync(userId, cancellationToken);

            if (skillGap is null)
            {
                return null;
            }

            decimal? cvQualityScore = await _context.CVAnalyses.AsNoTracking().Where(analysis => analysis.CV.UserID == userId && analysis.CV.IsCurrent).OrderByDescending(analysis => analysis.AnalysedAt).Select(analysis => (decimal?)analysis.QualityScore).FirstOrDefaultAsync(cancellationToken);

            if (!cvQualityScore.HasValue)
            {
                return null;
            }

            decimal careerReadinessScore = Math.Round((cvQualityScore.Value + skillGap.SkillMatchPercentage) / 2m, 2);

            return new CareerReadinessDto
            {
                CareerReadinessScore = careerReadinessScore,
                CVQualityScore = cvQualityScore.Value,
                SkillMatchPercentage = skillGap.SkillMatchPercentage,
                TargetCareer = new CareerReadinessCareerDto
                {
                    CareerID = skillGap.Career.CareerID,
                    CareerName = skillGap.Career.CareerName
                }
            };
        }
    }
}
