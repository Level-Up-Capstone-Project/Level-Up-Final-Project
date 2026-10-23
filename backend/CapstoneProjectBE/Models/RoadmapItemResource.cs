namespace CapstoneProjectBE.Models
{
    public class RoadmapItemResource
    {
        public Guid RoadmapItemResourceID { get; set; }

        public Guid RoadmapItemID { get; set; }
        public RoadmapItem RoadmapItem { get; set; } = null!;

        public Guid ResourceID { get; set; }
        public LearningResource LearningResource { get; set; } = null!;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

    }
}