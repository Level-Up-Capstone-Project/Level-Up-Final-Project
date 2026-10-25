using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace CapstoneProjectBE.Services
{
    /// <summary>
    /// Coordinates Verification Code service application behavior.
    /// </summary>
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly IPasswordHasher<EmailVerification> _codeHasher;

        public VerificationCodeService(IPasswordHasher<EmailVerification> codeHasher)
        {
            _codeHasher = codeHasher;
        }

        public string GenerateCode()
        {
            int number = RandomNumberGenerator.GetInt32(0, 1_000_000);
            return number.ToString("D6");
        }

        public string HashVerificationCode(EmailVerification verification, string code)
        {
            return _codeHasher.HashPassword(verification, code);
        }

        public bool VerifyVerificationCode(EmailVerification verification, string codeHash, string providedCode)
        {
            PasswordVerificationResult result = _codeHasher.VerifyHashedPassword(verification, codeHash, providedCode);
            return result != PasswordVerificationResult.Failed;
        }
    }
}
