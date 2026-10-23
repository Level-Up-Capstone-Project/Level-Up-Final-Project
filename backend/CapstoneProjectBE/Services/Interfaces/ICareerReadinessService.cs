using CapstoneProjectBE.DTOs.CareerReadiness;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface ICareerReadinessService
    {
        Task<CareerReadinessDto?> GetCareerReadinessAsync(Guid userId, CancellationToken cancellationToken);
    }
}
