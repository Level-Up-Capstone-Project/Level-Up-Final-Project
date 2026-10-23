namespace CapstoneProjectBE.Models
{
    //a job listing
    public class Job
    {
        public Guid JobID { get; set; }

        public Guid MappedCareerID { get; set; }
        public Career MappedCareer { get; set; } = null!;

        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public string? Description { get; set; }

        public string? QualificationRequirement { get; set; }

        public int? ExperienceMin { get; set; }
        public int? ExperienceMax { get; set; }

        public string? JobType { get; set; }

        public string? ApplicationUrl { get; set; }

        public string? SourceName { get; set; }

        public string? SourceListingID { get; set; }

        public DateTime? PostedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? RetrievedAt { get; set; }

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<JobMatch> JobMatches { get; set; } = new List<JobMatch>();
    }
}
