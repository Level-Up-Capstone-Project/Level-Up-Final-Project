using CapstoneProjectBE.Models;

namespace CapstoneProjectBE.Services.Interfaces
{
    /// <summary>
    /// Defines the operations provided by the Verification Code service abstraction.
    /// </summary>
    public interface IVerificationCodeService
    {
        string GenerateCode();

        string HashVerificationCode(EmailVerification verification, string code);

        bool VerifyVerificationCode(EmailVerification verification, string codeHash, string providedCode    );
    }
}
