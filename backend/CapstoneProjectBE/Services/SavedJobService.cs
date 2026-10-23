using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Jobs;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CapstoneProjectBE.Models;
using Npgsql;

namespace CapstoneProjectBE.Services
{
    //user's saved jobs
    public class SavedJobService : ISavedJobService
    {
        private readonly AppDbContext _context;
        public SavedJobService(AppDbContext context)
        {
            _context = context;
        }

        //GET /api/saved-jobs
        public async Task<List<SavedJobDto>> GetSavedJobsAsync (Guid userId, CancellationToken cancellationToken)
        {
            return await _context.SavedJobs
            .AsNoTracking()
            .Where(savedJob => savedJob.UserID == userId)
            .OrderByDescending(savedJob => savedJob.SavedAt)
            .Select(savedJob => new SavedJobDto
            {
                SavedJobID = savedJob.SavedJobID,
                SavedAt = savedJob.SavedAt,
                Job = new JobSummaryDto
                {
                    JobID = savedJob.Job.JobID,
                    JobTitle = savedJob.Job.JobTitle,
                    CompanyName = savedJob.Job.CompanyName,
                    Location = savedJob.Job.Location,
                    JobType = savedJob.Job.JobType,
                    CareerName = savedJob.Job.MappedCareer.CareerName,
                    PostedAt = savedJob.Job.PostedAt
                }

            })
            .ToListAsync(cancellationToken);
        }

        //POST /api/saved-jobs/{jobID}
        public async Task<SavedJobDto?> SaveJobAsync (Guid userId, Guid jobId, CancellationToken cancellationToken)
        {
            var jobExists = await _context.Jobs.AnyAsync(job => job.JobID == jobId, cancellationToken);

            if (!jobExists)
            {
                return null;
            }

            var existing = await GetSavedJobAsync(userId, jobId, cancellationToken);

            if (existing is not null)
            {
                return existing;
            }

            var savedJob = new SavedJob
            {
                UserID = userId,
                JobID = jobId,
                SavedAt = DateTime.UtcNow
            };

            _context.SavedJobs.Add(savedJob);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception)
                when (exception.InnerException is PostgresException
                { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                _context.Entry(savedJob).State = EntityState.Detached;
            }

            return await GetSavedJobAsync(userId, jobId, cancellationToken);
        }

        //DELET /api/saved-jobs/{jobID}
        public async Task<bool> UnsavedJobAsync (Guid userId, Guid jobId, CancellationToken cancellationToken)
        {
            var savedJobs = await _context.SavedJobs
            .Where(savedJob => savedJob.UserID == userId && savedJob.JobID == jobId)
            .ToListAsync(cancellationToken);

            if (savedJobs.Count == 0)
            {
                return false;
            }

            _context.SavedJobs.RemoveRange(savedJobs);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        //reads one saved job back as Dto
        private async Task<SavedJobDto?> GetSavedJobAsync(Guid userId, Guid jobId, CancellationToken cancellationToken)
        {
            return await _context.SavedJobs
            .AsNoTracking()
            .Where(savedJob => savedJob.UserID == userId && savedJob.JobID == jobId)
            .Select(savedJob => new SavedJobDto
            {
                SavedJobID = savedJob.SavedJobID,
                SavedAt = savedJob.SavedAt,
                Job = new JobSummaryDto
                {
                    JobID = savedJob.Job.JobID,
                    JobTitle = savedJob.Job.JobTitle,
                    CompanyName = savedJob.Job.CompanyName,
                    Location = savedJob.Job.Location,
                    JobType = savedJob.Job.JobType,
                    CareerName = savedJob.Job.MappedCareer.CareerName,
                    PostedAt = savedJob.Job.PostedAt
                }
            })
            .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
