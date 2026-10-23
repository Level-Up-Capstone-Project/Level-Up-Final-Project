namespace CapstoneProjectBE.DTOs.Jobs
{
    public class JobMatchDto
    {
        public Guid JobMatchID { get; set; }
        public Guid JobID { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? JobType { get; set; }
        public string CareerName { get; set; } = string.Empty;

        public decimal SkillMatchScore { get; set; }
        public decimal EducationMatchScore { get; set; }
        public decimal ExperienceMatchScore { get; set; }
        public decimal CareerAlignmentScore { get; set; }
        public decimal OverallMatchScore { get; set; }
        public decimal MatchPercentage { get; set; }
        public DateTime CalculatedAt { get; set; }

        public List<JobSkillDto> MatchedSkills { get; set; } = new List<JobSkillDto>();

        public List<JobSkillDto> MissingSkills { get; set; } = new List<JobSkillDto>();
    }
}
