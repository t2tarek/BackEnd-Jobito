namespace CompanyDashboardAPI.Models;

public enum ApplicationStatus
{
    Pending,
    InReview,
    Shortlisted,
    Interviewed,
    Unsuitable,
    Declined,
    Hired
}

public enum ApplicantType
{
    FullTime,
    PartTime,
    Freelance,
    Internship
}
