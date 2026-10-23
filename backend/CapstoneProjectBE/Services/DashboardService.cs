using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Dashboard;
using CapstoneProjectBE.DTOs.CareerReadiness;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        private readonly ICareerReadinessService _careerReadinessService;

        public DashboardService(
            AppDbContext context,
            ICareerReadinessService careerReadinessService)
        {
            _context = context;
            _careerReadinessService = careerReadinessService;
        }

        public async Task<DashboardDto?> GetAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            bool userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(user => user.UserID == userId, cancellationToken);

            if (!userExists)
            {
                return null;
            }

            var profile = await _context.UserProfiles
                .AsNoTracking()
                .Where(item => item.UserID == userId)
                .Select(item => new
                {
                    Profile = new DashboardProfileDto
                    {
                        FName = item.FName,
                        LName = item.LName,
                        ProfileCompleted = item.ProfileCompleted
                    },
                    TargetCareer = item.TargetCareer == null
                        ? null
                        : new DashboardCareerDto
                        {
                            CareerID = item.TargetCareer.CareerID,
                            CareerName = item.TargetCareer.CareerName
                        }
                })
                .SingleOrDefaultAsync(cancellationToken);

            CareerReadinessDto? readiness = await _careerReadinessService
                .GetCareerReadinessAsync(userId, cancellationToken);

            // Document quality is available even before a target career is selected.
            decimal? cvQualityScore = await _context.CVAnalyses.AsNoTracking()
                .Where(analysis => analysis.CV.UserID == userId && analysis.CV.IsCurrent)
                .OrderByDescending(analysis => analysis.AnalysedAt)
                .Select(analysis => (decimal?)analysis.QualityScore)
                .FirstOrDefaultAsync(cancellationToken);

            DashboardCVDto? currentCV = await _context.CVs
                .AsNoTracking()
                .Where(cv => cv.UserID == userId && cv.IsCurrent)
                .Select(cv => new DashboardCVDto
                {
                    CVID = cv.CVID,
                    FileName = cv.FileName
                })
                .SingleOrDefaultAsync(cancellationToken);

            decimal roadmapProgress = await _context.LearningRoadmaps
                .AsNoTracking()
                .Where(roadmap => roadmap.UserID == userId)
                .OrderByDescending(roadmap => roadmap.UpdatedAt)
                .Select(roadmap => (decimal?)roadmap.ProgressPercentage)
                .FirstOrDefaultAsync(cancellationToken) ?? 0m;

            int savedResourcesCount = await _context.SavedResources
                .AsNoTracking()
                .CountAsync(saved => saved.UserID == userId, cancellationToken);

            int savedJobsCount = await _context.SavedJobs
                .AsNoTracking()
                .CountAsync(saved => saved.UserID == userId, cancellationToken);

            return new DashboardDto
            {
                Profile = profile?.Profile,
                TargetCareer = profile?.TargetCareer,
                CareerReadinessScore = readiness?.CareerReadinessScore,
                CVQualityScore = cvQualityScore,
                SkillMatchPercentage = readiness?.SkillMatchPercentage,
                RoadmapProgressPercentage = roadmapProgress,
                CurrentCV = currentCV,
                SavedJobsCount = savedJobsCount,
                SavedResourcesCount = savedResourcesCount
            };
        }
    }
}
