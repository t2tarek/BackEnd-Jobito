using CompanyDashboardAPI.DTOs;

namespace CompanyDashboardAPI.Services;

public interface ICompanyDashboardService
{
    DashboardStatsDto GetDashboardStats(DashboardPayloadDto payload);
    ApplicantSummaryDto GetApplicantSummary(DashboardPayloadDto payload);
    IEnumerable<JobUpdateDto> GetLatestJobUpdates(DashboardPayloadDto payload);
    JobListingStatsDto GetJobListingStats(DashboardPayloadDto payload);
}
