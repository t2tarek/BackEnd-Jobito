using CompanyDashboardAPI.DTOs;

namespace CompanyDashboardAPI.Services;

public interface IUserDashboardService
{
    Task<UserDashboardDto> GetUserDashboardMetricsAsync(string email, int daysBack);
}
