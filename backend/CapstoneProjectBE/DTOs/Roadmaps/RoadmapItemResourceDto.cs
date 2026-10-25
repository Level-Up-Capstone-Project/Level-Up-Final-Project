using CapstoneProjectBE.DTOs.Resources;

namespace CapstoneProjectBE.DTOs.Roadmaps
{
    //one resource linked to roadmap step
    //and whether user has marked it as completed
    /// <summary>
    /// Carries Roadmap Item Resource data between API clients and application services.
    /// </summary>
    public class RoadmapItemResourceDto
    {
        public Guid RoadmapItemResourceID { get; set;}

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

        public LearningResourceDto Resource { get; set; } = null!;
    }
}
