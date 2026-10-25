using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Common;
using CapstoneProjectBE.DTOs.Jobs;
using Microsoft.EntityFrameworkCore;


namespace CapstoneProjectBE.Services
{
    // job listin (paged + filtered) + job details
    //expired jobs hidden from list but viewable by ID
    /// <summary>
    /// Coordinates Job service application behavior.
    /// </summary>
    public class JobService : IJobService
    {
        private const int MaxPageSize = 50;

        private readonly AppDbContext _context;

        public JobService(AppDbContext context)
        {
            _context = context;
        }

        //GET /api/jobs
        public async Task<PagedResultDto<JobSummaryDto>> GetJobsAsync(JobQueryDto query, CancellationToken cancellationToken)
        {
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Clamp(query.PageSize, 1, MaxPageSize);

            var now = DateTime.UtcNow;

            //jobs that have not expired
            var jobs = _context.Jobs
            .AsNoTracking()
            .Where(job => job.ExpiresAt == null || job.ExpiresAt > now);

            if (query.CareerID != null)
            {
                jobs = jobs.Where(job => job.MappedCareerID == query.CareerID);
            }

            if (!string.IsNullOrWhiteSpace(query.Location))
            {
                jobs = jobs.Where(job => job.Location != null
                    && EF.Functions.ILike(job.Location, $"%{query.Location}%"));

            }

            if (!string.IsNullOrWhiteSpace(query.JobType))
            {
                jobs = jobs.Where(job => job.JobType != null
                    && EF.Functions.ILike(job.JobType, query.JobType));
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                jobs = jobs.Where(job =>
                EF.Functions.ILike(job.JobTitle, $"%{query.Search}%") || 
                EF.Functions.ILike(job.CompanyName, $"%{query.Search}%"));
            }

            var totalCount = await jobs.CountAsync(cancellationToken);

            var items = await jobs
            .OrderByDescending(job => job.PostedAt)
            .ThenBy(job => job.JobID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(job => new JobSummaryDto
            {
                JobID = job.JobID,
                JobTitle = job.JobTitle,
                CompanyName = job.CompanyName,
                Location = job.Location,
                JobType = job.JobType,
                CareerName = job.MappedCareer.CareerName,
                PostedAt = job.PostedAt
            })
            .ToListAsync(cancellationToken);

            return new PagedResultDto<JobSummaryDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        //GET /api/jobs/{id}
        public async Task<JobDto?> GetByIdAsync (Guid jobId, CancellationToken cancellationToken)
        {
            return await _context.Jobs
            .AsNoTracking()
            .Where(job => job.JobID == jobId)
            .Select(job => new JobDto
            {
                JobID = job.JobID,
                JobTitle = job.JobTitle,
                CompanyName = job.CompanyName,
                Location = job.Location,
                JobType = job.JobType,
                MappedCareerID = job.MappedCareerID,
                CareerName = job.MappedCareer.CareerName,
                Description = job.Description,
                QualificationRequirement = job.QualificationRequirement,
                ExperienceMin = job.ExperienceMin,
                ExperienceMax = job.ExperienceMax,
                ApplicationUrl = job.ApplicationUrl,
                SourceName = job.SourceName,
                PostedAt = job.PostedAt,
                ExpiresAt = job.ExpiresAt,
                Skills = job.JobSkills
                .OrderByDescending(jobSkill => jobSkill.ImportanceLevel)
                .Select(jobSkill => new JobSkillDto
                {
                    SkillID = jobSkill.SkillID,
                    SkillName = jobSkill.Skill.SkillName,
                    RequirementType = jobSkill.RequirementType,
                    ImportanceLevel = jobSkill.ImportanceLevel

                })
                .ToList()

            })
            .SingleOrDefaultAsync(cancellationToken);
        }

    }
}
