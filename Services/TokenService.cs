using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CompanyDashboardAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace CompanyDashboardAPI.Services;

public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        _config = config;
        var keyStr = _config["Jwt:Key"] ?? "ThisIsAVerySecretKeyForJobitoUnifiedBackend2026!";
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
    }

    public string CreateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("name", user.FullName ?? ""),
            new Claim("role", (user.Role ?? "").ToLower() == "company" ? "company" : "user"),
            new Claim("UserType", user.Role ?? "JobApplication"),
            new Claim("avatar", user.AvatarUrl ?? ""),
            new Claim("banner", user.BannerUrl ?? ""),
            new Claim("classification", user.Classification ?? ""),
            new Claim("location", user.Location ?? ""),
            new Claim("bio", user.Bio ?? ""),
            new Claim("phone", user.PhoneNumber ?? ""),
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
