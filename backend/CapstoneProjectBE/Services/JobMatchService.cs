using System.Text.RegularExpressions;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Jobs;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates Job Match service application behavior.
    /// </summary>
    public class JobMatchService : IJobMatchService
    {
        private readonly AppDbContext _context;

        public JobMatchService(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<JobMatchDto>> GetMatchesAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            // Recalculate on read so profile and skill changes cannot leave stale scores.
            return RecalculateAsync(userId, cancellationToken);
        }

        public async Task<JobMatchDto?> GetMatchForJobAsync(
            Guid userId,
            Guid jobId,
            CancellationToken cancellationToken)
        {
            var job = await JobsForMatching()
                .SingleOrDefaultAsync(job => job.JobID == jobId, cancellationToken);

            if (job is null)
            {
                return null;
            }

            var inputs = await GetUserMatchInputsAsync(userId, cancellationToken);
            var calculated = CalculateMatch(job, inputs);
            await StoreMatchesAsync(userId, [calculated], cancellationToken);

            return calculated.Dto;
        }

        public async Task<List<JobMatchDto>> RecalculateAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var jobs = await JobsForMatching()
                .Where(job => job.ExpiresAt == null || job.ExpiresAt > now)
                .ToListAsync(cancellationToken);

            var inputs = await GetUserMatchInputsAsync(userId, cancellationToken);
            var calculated = jobs
                .Select(job => CalculateMatch(job, inputs))
                .ToList();

            await StoreMatchesAsync(userId, calculated, cancellationToken);

            return calculated
                .Select(match => match.Dto)
                .OrderByDescending(match => match.OverallMatchScore)
                .ThenBy(match => match.JobTitle)
                .ToList();
        }

        private async Task<UserMatchInputs> GetUserMatchInputsAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var profile = await _context.UserProfiles
                .AsNoTracking()
                .Where(profile => profile.UserID == userId)
                .Select(profile => new
                {
                    profile.TargetCareerID,
                    profile.Qualification,
                    profile.ExperienceLevel
                })
                .SingleOrDefaultAsync(cancellationToken);

            var userSkills = await _context.UserSkills
                .AsNoTracking()
                .Where(userSkill => userSkill.UserID == userId)
                .Select(userSkill => new { userSkill.SkillID, userSkill.Status })
                .ToListAsync(cancellationToken);

            var weights = userSkills
                .Select(userSkill => new
                {
                    userSkill.SkillID,
                    Weight = UserSkillStatusResolver.ToMatchWeight(
                        userSkill.Status ?? UserSkillStatusResolver.Missing)
                })
                .GroupBy(userSkill => userSkill.SkillID)
                .ToDictionary(
                    group => group.Key,
                    group => group.Max(userSkill => userSkill.Weight));

            var targetCareerSkillIDs = profile?.TargetCareerID is Guid targetCareerId
                ? (await _context.CareerSkills
                    .AsNoTracking()
                    .Where(careerSkill => careerSkill.CareerID == targetCareerId)
                    .Select(careerSkill => careerSkill.SkillID)
                    .ToListAsync(cancellationToken))
                    .ToHashSet()
                : [];

            return new UserMatchInputs
            {
                SkillWeights = weights,
                Qualification = profile?.Qualification,
                ExperienceLevel = profile?.ExperienceLevel,
                TargetCareerID = profile?.TargetCareerID,
                TargetCareerSkillIDs = targetCareerSkillIDs
            };
        }

        private IQueryable<JobForMatching> JobsForMatching()
        {
            return _context.Jobs
                .AsNoTracking()
                .Select(job => new JobForMatching
                {
                    JobID = job.JobID,
                    MappedCareerID = job.MappedCareerID,
                    JobTitle = job.JobTitle,
                    CompanyName = job.CompanyName,
                    Location = job.Location,
                    JobType = job.JobType,
                    CareerName = job.MappedCareer.CareerName,
                    QualificationRequirement = job.QualificationRequirement,
                    ExperienceMin = job.ExperienceMin,
                    ExpiresAt = job.ExpiresAt,
                    Skills = job.JobSkills
                        .Select(jobSkill => new JobSkillDto
                        {
                            SkillID = jobSkill.SkillID,
                            SkillName = jobSkill.Skill.SkillName,
                            RequirementType = jobSkill.RequirementType,
                            ImportanceLevel = jobSkill.ImportanceLevel
                        })
                        .ToList()
                });
        }

        private static CalculatedJobMatch CalculateMatch(
            JobForMatching job,
            UserMatchInputs inputs)
        {
            var matched = job.Skills
                .Where(skill => inputs.SkillWeights.GetValueOrDefault(skill.SkillID) > 0)
                .OrderByDescending(skill => skill.ImportanceLevel)
                .ToList();

            var missing = job.Skills
                .Where(skill => inputs.SkillWeights.GetValueOrDefault(skill.SkillID) <= 0)
                .OrderByDescending(skill => skill.ImportanceLevel)
                .ToList();

            decimal totalImportance = job.Skills.Sum(skill => Math.Max(skill.ImportanceLevel, 1));
            decimal weightedMatch = job.Skills.Sum(skill =>
                Math.Max(skill.ImportanceLevel, 1)
                * inputs.SkillWeights.GetValueOrDefault(skill.SkillID));

            decimal skillScore = totalImportance == 0
                ? 100
                : Percentage(weightedMatch, totalImportance);

            decimal educationScore = CalculateEducationScore(
                inputs.Qualification,
                job.QualificationRequirement);

            decimal experienceScore = CalculateExperienceScore(
                inputs.ExperienceLevel,
                job.ExperienceMin);

            decimal careerScore = CalculateCareerAlignmentScore(job, inputs);
            decimal overallScore = Math.Round(
                (skillScore + educationScore + experienceScore + careerScore) / 4m,
                2,
                MidpointRounding.AwayFromZero);

            var calculatedAt = DateTime.UtcNow;

            return new CalculatedJobMatch
            {
                JobID = job.JobID,
                Dto = new JobMatchDto
                {
                    JobID = job.JobID,
                    JobTitle = job.JobTitle,
                    CompanyName = job.CompanyName,
                    Location = job.Location,
                    JobType = job.JobType,
                    CareerName = job.CareerName,
                    SkillMatchScore = skillScore,
                    EducationMatchScore = educationScore,
                    ExperienceMatchScore = experienceScore,
                    CareerAlignmentScore = careerScore,
                    OverallMatchScore = overallScore,
                    MatchPercentage = overallScore,
                    CalculatedAt = calculatedAt,
                    MatchedSkills = matched,
                    MissingSkills = missing
                }
            };
        }

        private async Task StoreMatchesAsync(
            Guid userId,
            IReadOnlyCollection<CalculatedJobMatch> calculated,
            CancellationToken cancellationToken)
        {
            if (calculated.Count == 0)
            {
                return;
            }

            var jobIds = calculated.Select(match => match.JobID).ToList();
            var stored = await _context.JobMatches
                .Where(match => match.UserID == userId && jobIds.Contains(match.JobID))
                .ToDictionaryAsync(match => match.JobID, cancellationToken);

            foreach (var result in calculated)
            {
                if (!stored.TryGetValue(result.JobID, out var match))
                {
                    match = new JobMatch
                    {
                        UserID = userId,
                        JobID = result.JobID
                    };
                    _context.JobMatches.Add(match);
                }

                match.SkillMatchScore = result.Dto.SkillMatchScore;
                match.EducationMatchScore = result.Dto.EducationMatchScore;
                match.ExperienceMatchScore = result.Dto.ExperienceMatchScore;
                match.CareerAlignmentScore = result.Dto.CareerAlignmentScore;
                match.OverallMatchScore = result.Dto.OverallMatchScore;
                match.CalculatedAt = result.Dto.CalculatedAt;
            }

            await _context.SaveChangesAsync(cancellationToken);

            foreach (var result in calculated)
            {
                result.Dto.JobMatchID = _context.JobMatches.Local
                    .Single(match => match.UserID == userId && match.JobID == result.JobID)
                    .JobMatchID;
            }
        }

        private static decimal CalculateEducationScore(
            string? qualification,
            string? requirement)
        {
            if (string.IsNullOrWhiteSpace(requirement))
            {
                return 100;
            }

            if (string.IsNullOrWhiteSpace(qualification))
            {
                return 0;
            }

            var normalizedQualification = qualification.Trim().ToLowerInvariant();
            var normalizedRequirement = requirement.Trim().ToLowerInvariant();

            if (normalizedQualification.Contains(normalizedRequirement)
                || normalizedRequirement.Contains(normalizedQualification))
            {
                return 100;
            }

            var requirementTokens = Tokenize(normalizedRequirement);
            if (requirementTokens.Count == 0)
            {
                return 100;
            }

            var qualificationTokens = Tokenize(normalizedQualification);
            int overlap = requirementTokens.Count(qualificationTokens.Contains);
            return Percentage(overlap, requirementTokens.Count);
        }

        private static decimal CalculateExperienceScore(
            string? experienceLevel,
            int? minimumYears)
        {
            if (minimumYears is null or <= 0)
            {
                return 100;
            }

            decimal? years = ExtractExperienceYears(experienceLevel);
            return years is null
                ? 0
                : Percentage(Math.Min(years.Value, minimumYears.Value), minimumYears.Value);
        }

        private static decimal CalculateCareerAlignmentScore(
            JobForMatching job,
            UserMatchInputs inputs)
        {
            if (inputs.TargetCareerID is null)
            {
                return 0;
            }

            if (inputs.TargetCareerID == job.MappedCareerID)
            {
                return 100;
            }

            decimal totalImportance = job.Skills.Sum(skill => Math.Max(skill.ImportanceLevel, 1));
            if (totalImportance == 0)
            {
                return 0;
            }

            decimal alignedImportance = job.Skills
                .Where(skill => inputs.TargetCareerSkillIDs.Contains(skill.SkillID))
                .Sum(skill => Math.Max(skill.ImportanceLevel, 1));

            return Percentage(alignedImportance, totalImportance);
        }

        private static HashSet<string> Tokenize(string value)
        {
            string[] ignoredWords = ["and", "the", "with", "for", "degree", "diploma"];
            return Regex.Matches(value, "[a-z0-9+#.]+")
                .Select(match => match.Value)
                .Where(token => token.Length > 2 && !ignoredWords.Contains(token))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private static decimal? ExtractExperienceYears(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var match = Regex.Match(value, @"\d+");
            if (match.Success && decimal.TryParse(match.Value, out decimal years))
            {
                return years;
            }

            string normalized = value.ToLowerInvariant();
            if (normalized.Contains("senior")) return 5;
            if (normalized.Contains("mid")) return 3;
            if (normalized.Contains("junior") || normalized.Contains("entry")) return 1;
            if (normalized.Contains("none") || normalized.Contains("student")) return 0;

            return null;
        }

        private static decimal Percentage(decimal numerator, decimal denominator)
        {
            if (denominator <= 0)
            {
                return 0;
            }

            return Math.Clamp(
                Math.Round(numerator * 100m / denominator, 2, MidpointRounding.AwayFromZero),
                0,
                100);
        }

        /// <summary>
        /// Coordinates Job For Matching application behavior.
        /// </summary>
        private sealed class JobForMatching
        {
            public Guid JobID { get; set; }
            public Guid MappedCareerID { get; set; }
            public string JobTitle { get; set; } = string.Empty;
            public string CompanyName { get; set; } = string.Empty;
            public string? Location { get; set; }
            public string? JobType { get; set; }
            public string CareerName { get; set; } = string.Empty;
            public string? QualificationRequirement { get; set; }
            public int? ExperienceMin { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public List<JobSkillDto> Skills { get; set; } = [];
        }

        /// <summary>
        /// Coordinates User Match Inputs application behavior.
        /// </summary>
        private sealed class UserMatchInputs
        {
            public Dictionary<Guid, decimal> SkillWeights { get; set; } = [];
            public string? Qualification { get; set; }
            public string? ExperienceLevel { get; set; }
            public Guid? TargetCareerID { get; set; }
            public HashSet<Guid> TargetCareerSkillIDs { get; set; } = [];
        }

        /// <summary>
        /// Coordinates Calculated Job Match application behavior.
        /// </summary>
        private sealed class CalculatedJobMatch
        {
            public Guid JobID { get; set; }
            public JobMatchDto Dto { get; set; } = null!;
        }
    }
}
