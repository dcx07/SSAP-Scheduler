using SSAPSchedulerMaui.Models;

namespace SSAPSchedulerMaui.Services;

public interface IBackendRunner
{
    Task<BackendRunResult> RunBackendAsync(string username, string password, CancellationToken cancellationToken = default);
}

public class BackendRunResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<Course> Courses { get; set; } = new();
}