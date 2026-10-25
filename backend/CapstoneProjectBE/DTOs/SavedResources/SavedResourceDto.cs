namespace CapstoneProjectBE.DTOs.SavedResources
{
    /// <summary>
    /// Carries Saved Resource data between API clients and application services.
    /// </summary>
    public class SavedResourceDto
    {
        public Guid SavedResourceID { get; set; }
        public Guid ResourceID { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public string CostType { get; set; } = string.Empty;
        public string EstDuration { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? SavedAt { get; set; }
    }
}
