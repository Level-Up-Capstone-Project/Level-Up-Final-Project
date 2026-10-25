namespace CapstoneProjectBE.DTOs.CareerReadiness
{
    /// <summary>
    /// Carries Career Readiness data between API clients and application services.
    /// </summary>
    public class CareerReadinessDto
    {
        public decimal CareerReadinessScore { get; set; }
        public decimal CVQualityScore { get; set; }
        public decimal SkillMatchPercentage { get; set; }
        public CareerReadinessCareerDto TargetCareer { get; set; } = new CareerReadinessCareerDto();
    }
}
