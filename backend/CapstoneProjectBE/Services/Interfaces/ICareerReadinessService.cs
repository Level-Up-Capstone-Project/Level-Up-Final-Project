using CapstoneProjectBE.DTOs.CareerReadiness;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Career Readiness service abstraction.
    /// </summary>
    public interface ICareerReadinessService
    {
        Task<CareerReadinessDto?> GetCareerReadinessAsync(Guid userId, CancellationToken cancellationToken);
    }
}
