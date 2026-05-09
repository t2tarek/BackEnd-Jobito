namespace CompanyDashboardAPI.DTOs;

public class CompanyDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Industry { get; set; }
    public string? TechStack { get; set; }
    public string? SocialLinks { get; set; }
    public string? EstablishedYear { get; set; }
    public string? FoundedDay { get; set; }
    public string? FoundedMonth { get; set; }
    public string? Employees { get; set; }
    public string? VerificationStatus { get; set; }
    public List<JobDto>? Jobs { get; set; }
    public string? Benefits { get; set; }
    public string? Classification { get; set; }
}

public class UpdateCompanyDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Industry { get; set; }
    public string? TechStack { get; set; }
    public string? SocialLinks { get; set; }
    public string? EstablishedYear { get; set; }
    public string? FoundedDay { get; set; }
    public string? FoundedMonth { get; set; }
    public string? Employees { get; set; }
    public string? Benefits { get; set; }
    public string? Classification { get; set; }
}
