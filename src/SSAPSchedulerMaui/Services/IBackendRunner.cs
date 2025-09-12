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
    
    // Diagnostics fields for UI binding
    public string BackendDirResolved { get; set; } = string.Empty;
    public bool ConfigFound { get; set; }
    public bool ConfigWritten { get; set; }
    public bool ExeFound { get; set; }
    public bool ProcessStarted { get; set; }
    public bool ProcessExited { get; set; }
    public int? ExitCode { get; set; }
    public string StdErrSummary { get; set; } = string.Empty;
    public bool ScheduleJsonFound { get; set; }
    public bool ScheduleParsed { get; set; }
}