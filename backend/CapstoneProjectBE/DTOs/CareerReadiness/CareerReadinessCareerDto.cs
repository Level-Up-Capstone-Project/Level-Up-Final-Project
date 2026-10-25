namespace CapstoneProjectBE.DTOs.CareerReadiness
{
    /// <summary>
    /// Carries Career Readiness Career data between API clients and application services.
    /// </summary>
    public class CareerReadinessCareerDto
    {
        public Guid CareerID { get; set; }
        public string CareerName { get; set; } = string.Empty;
    }
}
