using SSAPSchedulerMaui.Models;

namespace SSAPSchedulerMaui.Services;

public interface IBackendService
{
    Task SaveCredentialsAsync(string username, string password);
    Task<List<Course>> GetTodayCoursesAsync();
    Task<string> GetBackendDirectoryAsync();
}