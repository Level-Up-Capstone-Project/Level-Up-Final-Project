using CapstoneProjectBE.DTOs.Dashboard;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto?> GetAsync(Guid userId, CancellationToken cancellationToken);
    }
}
