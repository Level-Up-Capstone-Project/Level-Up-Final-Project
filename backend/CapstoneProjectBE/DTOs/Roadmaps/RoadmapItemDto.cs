namespace CapstoneProjectBE.DTOs.Roadmaps
{
    //one step in roadmap
    public class RoadmapItemDto
    {
        public Guid RoadmapItemID { get; set; }
        public Guid RoadmapID { get; set; }
        public Guid SkillID { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int StepOrder { get; set; }

        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly? TargetDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
