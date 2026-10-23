
//the steps in the user's plan
namespace CapstoneProjectBE.Models
{
    public class RoadmapItem
    {
        public Guid RoadmapItemID { get; set; }

        public Guid RoadmapID { get; set; }
        public LearningRoadmap Roadmap { get; set; } = null!;

        public Guid SkillID { get; set; }
        public Skill Skill { get; set; } = null!;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int StepOrder { get; set; }

        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateOnly? TargetDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ICollection<RoadmapItemResource> Resources { get; set; } = new List<RoadmapItemResource>();


    }
}
