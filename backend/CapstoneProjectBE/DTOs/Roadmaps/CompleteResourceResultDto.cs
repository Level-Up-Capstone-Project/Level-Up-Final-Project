namespace CapstoneProjectBE.DTOs.Roadmaps
{
    //result of marking resource complete
    /// <summary>
    /// Carries Complete Resource Result data between API clients and application services.
    /// </summary>
    public class CompleteResourceResultDto
    {
        public Guid RoadmapItemResourceID { get; set; }

        public Guid RoadmapItemID { get; set; }

        public Guid RoadmapID { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string ItemStatus { get; set; } = string.Empty;

        public decimal ProgressPercentage { get; set; }
    }
}
