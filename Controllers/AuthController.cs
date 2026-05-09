using CompanyDashboardAPI.DTOs;
using CompanyDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyDashboardAPI.Controllers;
using Microsoft.AspNetCore.Identity;
using CompanyDashboardAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _authService = authService;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("debug-login")]
    public async Task<IActionResult> DebugLogin(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return NotFound();
        var token = _tokenService.CreateToken(user);
        return Ok(new { access_token = token });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyOtpDto dto)
    {
        try
        {
            var result = await _authService.VerifyEmailAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode([FromBody] ResendOtpDto dto)
    {
        try
        {
            var result = await _authService.ResendCodeAsync(dto.Email);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        try
        {
            var result = await _authService.ForgotPasswordAsync(dto.Email);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// DEV ONLY: Force-activate a user account by email (bypasses email verification)
    /// Usage: POST /api/auth/dev/activate  Body: { "email": "user@example.com" }
    /// </summary>
    [HttpPost("dev/activate")]
    public async Task<IActionResult> DevActivate([FromBody] ForgotPasswordDto dto)
    {
        var userManager = HttpContext.RequestServices
            .GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<CompanyDashboardAPI.Models.ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null) return NotFound(new { message = $"User '{dto.Email}' not found in database" });

        var wasActive = user.IsActive;
        user.IsActive = true;
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(new { message = "Failed to activate account", errors = result.Errors });

        return Ok(new
        {
            message = $"Account '{dto.Email}' activated successfully",
            previousStatus = wasActive ? "was already active" : "was inactive - now activated",
            role = user.Role
        });
    }

    /// <summary>
    /// DEV ONLY: Check user status in DB
    /// </summary>
    [HttpGet("dev/status/{email}")]
    public async Task<IActionResult> DevStatus(string email)
    {
        var userManager = HttpContext.RequestServices
            .GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<CompanyDashboardAPI.Models.ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return NotFound(new { message = $"'{email}' not found" });

        return Ok(new
        {
            email = user.Email,
            role = user.Role,
            isActive = user.IsActive,
            hasPassword = !string.IsNullOrEmpty(user.PasswordHash),
            classification = user.Classification
        });
    }
}
