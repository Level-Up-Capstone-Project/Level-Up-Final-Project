namespace CapstoneProjectBE.DTOs.Jobs
{
    //saved job with job card details
    public class SavedJobDto
    {
        public Guid SavedJobID { get; set; }
        public DateTime SavedAt { get; set; }
        public JobSummaryDto Job{ get; set; } = null!;
    }
}