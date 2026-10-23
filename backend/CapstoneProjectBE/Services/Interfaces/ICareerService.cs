using CapstoneProjectBE.DTOs.Careers;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface ICareerService
    {
        Task<IReadOnlyList<CareerDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<CareerDto?> GetByIdAsync(Guid careerId, CancellationToken cancellationToken);
        Task<IReadOnlyList<CareerSkillDto>?> GetSkillsAsync(Guid careerId, CancellationToken cancellationToken);
    }
}
