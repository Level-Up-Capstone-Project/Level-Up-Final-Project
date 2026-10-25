namespace CapstoneProjectBE.DTOs.Roadmaps
{
    //roadmap with its steps
    /// <summary>
    /// Carries Roadmap data between API clients and application services.
    /// </summary>
    public class RoadmapDto
    {
        public Guid RoadmapID { get; set; }
        public Guid UserID { get; set; }
        public Guid CareerID { get; set; }

        public string CareerName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        
        public decimal ProgressPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<RoadmapItemDto> Items { get; set; } = new();
    }
}
