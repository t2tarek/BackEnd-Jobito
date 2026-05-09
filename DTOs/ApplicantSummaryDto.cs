namespace CompanyDashboardAPI.DTOs;

public class ApplicantSummaryDto
{
    public int FullTime { get; set; }
    public int Internship { get; set; }
    public int Contract { get; set; }
    public int PartTime { get; set; }
    public int Remote { get; set; }
    public int Total => FullTime + Internship + Contract + PartTime + Remote;
}
