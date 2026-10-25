namespace CapstoneProjectBE.DTOs.Jobs
{
    /// <summary>
    /// Carries Job data between API clients and application services.
    /// </summary>
    public class JobDto
    {
        public Guid JobID { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public string? JobType { get; set; }

        public Guid MappedCareerID { get; set; }
        public string CareerName { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? QualificationRequirement { get; set; }

        public string? ApplicationUrl { get; set; }
        public string? SourceName { get; set; }

        public int? ExperienceMin { get; set; }
        public int? ExperienceMax { get; set; }

        public DateTime? PostedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public List<JobSkillDto> Skills { get; set; } = new List<JobSkillDto>();
    }
}
