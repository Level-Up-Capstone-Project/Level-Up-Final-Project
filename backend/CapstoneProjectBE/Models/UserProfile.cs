namespace CapstoneProjectBE.Models
{
    /// <summary>
    /// Represents the persisted User Profile domain entity.
    /// </summary>
    public class UserProfile
    {
        public Guid ProfileID { get; set; }
        public Guid UserID { get; set; }
        public Guid? TargetCareerID { get; set; }
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public string? ExperienceLevel { get; set; } = string.Empty; //allowed to be empty during onboarding
        public string? Interests { get; set; } = string.Empty; //allowed to be empty during onboarding
        public string? Industry { get; set; }
        public string? Phone { get; set; }
        public string[] TargetRoles { get; set; } = Array.Empty<string>();
        public string? WorkAreasJson { get; set; }
        public bool ProfileCompleted { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User User { get; set; } = null!;
        public Career? TargetCareer { get; set; }
    }
}
