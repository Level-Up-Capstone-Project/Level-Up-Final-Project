namespace CapstoneProjectBE.DTOs.Jobs
{
    public class JobSummaryDto
    {
        public Guid JobID { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? JobType { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public DateTime? PostedAt { get; set; }
    }
}
