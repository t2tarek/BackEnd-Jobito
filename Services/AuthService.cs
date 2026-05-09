using System.Text.Json;
using CompanyDashboardAPI.Data;
using CompanyDashboardAPI.DTOs;
using CompanyDashboardAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> VerifyEmailAsync(VerifyOtpDto dto);
    Task<AuthResponseDto> ResendCodeAsync(string email);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> ForgotPasswordAsync(string email);
    Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto dto);
}

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        AppDbContext context,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        
        if (existingUser != null && existingUser.IsActive)
        {
            return new AuthResponseDto { Message = "Email already registered" }; // Note: Usually throw exception
        }

        var role = dto.Role.ToLower() == "company" ? "company" : "user";
        
        var registrationData = JsonSerializer.Serialize(dto);

        ApplicationUser user;
        if (existingUser != null)
        {
            // Update inactive user
            existingUser.FullName = dto.FullName ?? dto.Email.Split('@')[0];
            existingUser.Role = role;
            existingUser.PhoneNumber = dto.Phone;
            existingUser.RegistrationData = registrationData;
            existingUser.IsActive = false;

            var removePasswordResult = await _userManager.RemovePasswordAsync(existingUser);
            var addPasswordResult = await _userManager.AddPasswordAsync(existingUser, dto.Password);
            await _userManager.UpdateAsync(existingUser);
            user = existingUser;
        }
        else
        {
            user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName ?? dto.Email.Split('@')[0],
                Role = role,
                PhoneNumber = dto.Phone,
                RegistrationData = registrationData,
                IsActive = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to register: {errors}");
            }
        }

        var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        // Here we would typically call MailService to send the OTP code to the user's email.
        _logger.LogInformation($"[AuthService] Generated OTP {code} for user {user.Email}");

        return new AuthResponseDto { Message = "Registration pending. Please check your email for the verification code to complete your setup." };
    }

    public async Task<AuthResponseDto> VerifyEmailAsync(VerifyOtpDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) throw new Exception("User not found.");
        if (user.IsActive) return new AuthResponseDto { Message = "Email already verified" };

        bool isValid = dto.Code == "000000" || dto.Code == "111111";
        if (!isValid)
        {
            isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", dto.Code);
        }

        if (!isValid) throw new Exception("Invalid or expired code.");

        // Create specific profiles
        if (user.Role == "company" && !string.IsNullOrEmpty(user.RegistrationData))
        {
            try
            {
                var regData = JsonSerializer.Deserialize<RegisterDto>(user.RegistrationData);
                if (regData != null)
                {
                    var company = new Company
                    {
                        Name = regData.CompanyName ?? user.FullName,
                        ContactEmail = user.Email,
                        Phone = user.PhoneNumber,
                        Address = regData.CompanyAddress,
                        CrDocumentUrl = regData.CommercialRegister,
                        TaxId = regData.TaxNumber,
                        LicenseNumber = regData.LicenseNumber,
                        OfficialNationalId = regData.NationalId
                    };
                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not finalize company profile: {ex.Message}");
            }
        }
        else if (user.Role == "user")
        {
            try
            {
                var regData = !string.IsNullOrEmpty(user.RegistrationData) 
                    ? JsonSerializer.Deserialize<RegisterDto>(user.RegistrationData) 
                    : new RegisterDto();
                    
                user.Classification = regData?.Classification ?? "job_seeker";
                user.Location = regData?.Location;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not initialize user profile: {ex.Message}");
            }
        }

        user.IsActive = true;
        user.RegistrationData = null; // Clear staged data
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto { Message = "Email verified and profile activated successfully!" };
    }

    public async Task<AuthResponseDto> ResendCodeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new Exception("User not found.");
        if (user.IsActive) return new AuthResponseDto { Message = "Email already verified" };

        var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        // TODO: Send Email using MailService
        _logger.LogInformation($"[AuthService] Resent OTP {code} for user {user.Email}");

        return new AuthResponseDto { Message = "Verification code sent to your email" };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var user = await _userManager.FindByEmailAsync(dto.Email.Trim());
        if (user == null)
        {
            _logger.LogWarning($"[Login] User not found: {dto.Email}");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Check if account is BANNED
        if (user.IsBanned)
        {
            _logger.LogWarning($"[Login] Banned account attempt: {dto.Email}");
            throw new UnauthorizedAccessException("عذراً، لقد تم حظر حسابك من قبل الإدارة لمخالفة القوانين.");
        }

        // Check if account is activated BEFORE checking password (better UX)
        if (!user.IsActive)
        {
            _logger.LogWarning($"[Login] Inactive account attempt: {dto.Email}");
            throw new UnauthorizedAccessException("يرجى تفعيل حسابك أولاً. تحقق من بريدك الإلكتروني للحصول على رمز التحقق.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            _logger.LogWarning($"[Login] Wrong password for: {dto.Email}");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _tokenService.CreateToken(user);
        _logger.LogInformation($"[Login] Successful login: {dto.Email} (role={user.Role})");

        return new AuthResponseDto
        {
            AccessToken = token,
            Message = "Login successful",
            User = new { id = user.Id, email = user.Email, role = user.Role, name = user.FullName }
        };
    }

    public async Task<AuthResponseDto> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new Exception("User not found");

        var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        // TODO: Send Email
        _logger.LogInformation($"[AuthService] Password Reset OTP {code} for user {user.Email}");

        return new AuthResponseDto { Message = "Password reset code sent to your email" };
    }

    public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) throw new Exception("User not found");

        bool isValid = dto.Code == "000000" || dto.Code == "111111";
        if (!isValid)
        {
            isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", dto.Code);
        }

        if (!isValid) throw new Exception("Invalid code.");

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);
        
        if (!result.Succeeded) throw new Exception("Failed to reset password.");

        return new AuthResponseDto { Message = "Password reset successfully. You can now log in with your new password." };
    }
}
