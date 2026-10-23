using Microsoft.AspNetCore.Mvc;
using CapstoneProjectBE.DTOs.Auth;
using CapstoneProjectBE.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;


namespace CapstoneProjectBE.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [EnableRateLimiting("authentication")]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            if (request.TargetRoles?.Any(role => string.IsNullOrWhiteSpace(role) || role.Trim().Length > 100) == true)
                return BadRequest(new {message = "Target roles must be no longer than 100 characters."});

            RegisterResponse? response = await _authService.RegisterAsync(request, cancellationToken);

            if (response is null)
            {
                return Conflict(new
                {
                    message = "Email already exists."
                });
            }

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [EnableRateLimiting("password-recovery")]
        [HttpPost("password-reset/request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetCodeRequest request, CancellationToken cancellationToken)
        {
            await _authService.RequestPasswordResetAsync(request, cancellationToken);
            return Ok(new {message = "If an active account exists for that email, a password reset code has been sent."});
        }

        [EnableRateLimiting("password-recovery")]
        [HttpPost("password-reset/complete")]
        public async Task<IActionResult> CompletePasswordReset([FromBody] CompletePasswordResetRequest request, CancellationToken cancellationToken)
        {
            bool reset = await _authService.ResetPasswordAsync(request, cancellationToken);
            return reset
                ? Ok(new {message = "Your password has been reset. You can now sign in."})
                : BadRequest(new {message = "The password reset code is invalid or expired."});
        }

        [EnableRateLimiting("authentication")]
        [HttpPost("verify-email")]
        public async Task<ActionResult<VerifyEmailResponse>> VerifyEmail([FromBody]  VerifyEmailRequest request, CancellationToken cancellationToken)
        {
            VerifyEmailResponse? response = await _authService.VerifyEmailAsync(request, cancellationToken);

            if(response is null)
            {
                return BadRequest(new
                {
                    message = "Invalid/expired verification code."
                });
            }

            return Ok(response);
        }

        [EnableRateLimiting("authentication")]
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request, CancellationToken cancellationToken)
        {
            await _authService.ResendVerificationAsync(request, cancellationToken);

            return Ok(new
            {
                message = "Verification code has been sent."
            });
        }

        [EnableRateLimiting("authentication")]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            LoginResponse? response = await _authService.LoginAsync(request, cancellationToken);

            if(response is null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<AuthUserDto>> GetCurrentUser(CancellationToken cancellationToken)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            AuthUserDto? user = await _authService.GetCurrentUserAsync(userId, cancellationToken);

            if(user is null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }
    }
}
