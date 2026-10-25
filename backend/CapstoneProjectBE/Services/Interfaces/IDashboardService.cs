using CapstoneProjectBE.DTOs.Dashboard;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Dashboard service abstraction.
    /// </summary>
    public interface IDashboardService
    {
        Task<DashboardDto?> GetAsync(Guid userId, CancellationToken cancellationToken);
    }
}
