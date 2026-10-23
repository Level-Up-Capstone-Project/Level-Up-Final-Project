using CapstoneProjectBE.Models;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface IVerificationCodeService
    {
        string GenerateCode();

        string HashVerificationCode(EmailVerification verification, string code);

        bool VerifyVerificationCode(EmailVerification verification, string codeHash, string providedCode    );
    }
}
