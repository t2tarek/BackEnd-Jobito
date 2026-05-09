using System.Text.Json.Serialization;

namespace CompanyDashboardAPI.DTOs;

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "user"; // "user" or "company"
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    // Add additional registration fields that get serialized into RegistrationData
    public string? CompanyName { get; set; }
    public string? CompanyAddress { get; set; }
    public string? TaxNumber { get; set; }
    public string? LicenseNumber { get; set; }
    public string? CommercialRegister { get; set; }
    public string? NationalId { get; set; }
    public string? Classification { get; set; }
    public string? Location { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class VerifyOtpDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class ResendOtpDto
{
    public string Email { get; set; } = string.Empty;
}

public class GoogleLoginDto
{
    public string Token { get; set; } = string.Empty;
}

public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
    public object? User { get; set; }
}
