using System.Security.Claims;
using CompanyDashboardAPI.DTOs;
using CompanyDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CompanyDashboardAPI.Models;

namespace CompanyDashboardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompaniesService _companiesService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public CompaniesController(ICompaniesService companiesService, UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _companiesService = companiesService;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        var companies = await _companiesService.GetAllCompaniesAsync();
        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompanyById(long id)
    {
        var company = await _companiesService.GetCompanyByIdAsync(id);
        if (company == null) return NotFound(new { message = "Company not found" });

        return Ok(company);
    }

    [Authorize(Roles = "company")]
    [HttpGet("my/profile")]
    [HttpGet("my-company")]
    public async Task<IActionResult> GetMyCompany()
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var company = await _companiesService.GetCompanyByUserIdAsync(userId);
        if (company == null) return NotFound(new { message = "Company profile not found" });

        return Ok(company);
    }

    [Authorize(Roles = "company")]
    [HttpGet("my/dashboard-summary")]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var summary = await _companiesService.GetDashboardSummaryAsync(userId);
        if (summary == null) return NotFound(new { message = "Company profile not found" });

        return Ok(summary);
    }

    [Authorize(Roles = "company")]
    [HttpPatch("my/profile")]
    [HttpPut("my-company")]
    public async Task<IActionResult> UpdateMyCompany([FromBody] UpdateCompanyDto dto)
    {
        try
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var company = await _companiesService.GetCompanyByUserIdAsync(userId);
            if (company == null) return NotFound(new { message = "Company profile not found" });

            var success = await _companiesService.UpdateCompanyAsync(company.Id, dto);
            if (!success) return BadRequest(new { message = "Failed to update company profile" });

            // Sync User's FullName and AvatarUrl with the Company's Name and LogoUrl
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(dto.Name)) user.FullName = dto.Name;
                if (!string.IsNullOrEmpty(dto.LogoUrl)) user.AvatarUrl = dto.LogoUrl;
                if (dto.Classification != null) user.Classification = dto.Classification;
                if (dto.Address != null) user.Location = dto.Address;
                if (dto.Description != null) user.Bio = dto.Description;
                if (dto.Phone != null) user.PhoneNumber = dto.Phone;
                await _userManager.UpdateAsync(user);
            }

            var updatedCompany = await _companiesService.GetCompanyByIdAsync(company.Id);

            if (user != null)
            {
                var newToken = _tokenService.CreateToken(user);
                return Ok(new { access_token = newToken, profile = updatedCompany });
            }

            return Ok(updatedCompany);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
