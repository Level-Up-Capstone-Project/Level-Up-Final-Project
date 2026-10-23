
//user's plan for one career
namespace CapstoneProjectBE.Models
{
    public class LearningRoadmap
    {
        public Guid RoadmapID { get; set; }

       public Guid CareerID{ get; set; }
       public Career Career {get; set; } = null!;

       public Guid UserID { get; set; }
       public User User { get; set; } = null!;

       public string Title { get; set; } = string.Empty;

       public decimal ProgressPercentage { get; set; } 

       public DateTime CreatedAt { get; set; }

       public DateTime UpdatedAt { get; set; }

       public ICollection<RoadmapItem> Items { get; set; } = new List<RoadmapItem>();
       


    }
}