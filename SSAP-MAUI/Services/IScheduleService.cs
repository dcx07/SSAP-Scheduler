using SSAP_MAUI.Models;

namespace SSAP_MAUI.Services;

public interface IScheduleService
{
    Task<bool> SaveConfigAsync(string username, string password);
    Task<List<Course>> GetTodayScheduleAsync();
    Task<bool> RunBackendAsync();
    string GetBackendDirectory();
}