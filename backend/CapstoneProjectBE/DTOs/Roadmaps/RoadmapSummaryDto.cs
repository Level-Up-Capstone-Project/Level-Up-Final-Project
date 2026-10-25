namespace CapstoneProjectBE.DTOs.Roadmaps
{
    //roadmap without its steps, for lists
    /// <summary>
    /// Carries Roadmap Summary data between API clients and application services.
    /// </summary>
    public class RoadmapSummaryDto
    {
        public Guid RoadmapID { get; set; }
        public Guid CareerID { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal ProgressPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
