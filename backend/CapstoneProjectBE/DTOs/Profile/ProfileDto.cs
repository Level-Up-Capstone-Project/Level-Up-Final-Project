namespace CapstoneProjectBE.DTOs.Profile
{
    /// <summary>
    /// Carries Profile data between API clients and application services.
    /// </summary>
    public class ProfileDto
    {
        public Guid ProfileID { get; set; }
        public Guid UserID { get; set; }
        public Guid? TargetCareerID { get; set; }
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public string? ExperienceLevel { get; set; }
        public string? Interests { get; set; }
        public string? Industry { get; set; }
        public string? Phone { get; set; }
        public string[] TargetRoles { get; set; } = Array.Empty<string>();
        public IReadOnlyList<WorkAreaDto> WorkAreas { get; set; } = Array.Empty<WorkAreaDto>();
        public bool ProfileCompleted { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
