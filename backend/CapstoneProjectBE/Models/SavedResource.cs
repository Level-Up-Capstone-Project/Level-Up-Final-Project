namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted Saved Resource domain entity.
    /// </summary>
    public class SavedResource
    {
        public Guid SavedResourceID { get; set; }
        public Guid UserID { get; set; }
        public Guid ResourceID { get; set; }
        public DateTime? SavedAt { get; set; }
        public User User { get; set; } = null!;
        public LearningResource Resource { get; set; } = null!;
    }
}
