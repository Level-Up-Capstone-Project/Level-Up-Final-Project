namespace CapstoneProjectBE.DTOs.Jobs
{
    //query parameters for GET /api/jobs
    //optional
    public class JobQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public Guid? CareerID { get; set; }
        public string? Location { get; set; }
        public string? JobType { get; set; }
        public string? Search { get; set; }
    }
}