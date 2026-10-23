using CapstoneProjectBE.Models;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
