using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates Jwt Token service application behavior.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            string issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing");
            string audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is missing");
            string signingKey = _configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT signing key is missing");

            if (!int.TryParse(_configuration["Jwt:ExpiryMinutes"], out int expiryMinutes) || expiryMinutes <= 0)
            {
                throw new InvalidOperationException("JWT expiry must be a positive number.");
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            byte[] keyBytes = Convert.FromBase64String(signingKey);

            var securityKey = new SymmetricSecurityKey(keyBytes);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            DateTime now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims:  claims,
                notBefore: now,
                expires: now.AddMinutes(expiryMinutes),
                signingCredentials:  credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
