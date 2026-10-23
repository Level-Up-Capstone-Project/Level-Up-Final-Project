namespace CapstoneProjectBE.DTOs.CareerReadiness
{
    public class CareerReadinessDto
    {
        public decimal CareerReadinessScore { get; set; }
        public decimal CVQualityScore { get; set; }
        public decimal SkillMatchPercentage { get; set; }
        public CareerReadinessCareerDto TargetCareer { get; set; } = new CareerReadinessCareerDto();
    }
}
