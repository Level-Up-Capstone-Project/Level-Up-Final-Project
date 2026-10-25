using CapstoneProjectBE.Models;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Jwt Token service abstraction.
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
