namespace CapstoneProjectBE.DTOs.Resources
{
    /// <summary>
    /// Carries Learning Resource data between API clients and application services.
    /// </summary>
    public class LearningResourceDto
    {
        public Guid ResourceID { get; set; }

        public Guid SkillID { get; set; }
        
        public string ResourceName { get; set; } = string.Empty;

        public string ResourceType { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public string Provider { get; set; } = string.Empty;

        public string? ProviderResourceID { get; set; }


        public string DifficultyLevel { get; set; } = string.Empty;

        public string CostType { get; set; } = string.Empty;

        public string EstDuration { get; set; } = string.Empty;

         public string Description { get; set; } = string.Empty;


    }
}
