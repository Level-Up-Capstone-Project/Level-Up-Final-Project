namespace CapstoneProjectBE.Models
{
    //job a user bookmarked
    public class SavedJob
    {
        public Guid SavedJobID { get; set; }

        public Guid UserID { get; set; }
        public User User { get; set; } = null!;

        public Guid JobID { get; set; }
        public Job Job { get; set; } = null!;

        public DateTime SavedAt { get; set; }
    }
}