namespace CapstoneProjectBE.DTOs.Auth
{
    public enum LoginStatus
    {
        Success,
        InvalidCredentials,
        VerificationRequired
    }

    public sealed class LoginResult
    {
        public LoginStatus Status { get; init; }
        public LoginResponse? Response { get; init; }

        public static LoginResult Success(LoginResponse response) =>
            new() {Status = LoginStatus.Success, Response = response};

        public static LoginResult InvalidCredentials() =>
            new() {Status = LoginStatus.InvalidCredentials};

        public static LoginResult VerificationRequired() =>
            new() {Status = LoginStatus.VerificationRequired};
    }
}
