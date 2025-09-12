using SSAPSchedulerMaui.Models;

namespace SSAPSchedulerMaui.Services;

public class StubBackendRunner : IBackendRunner
{
    public async Task<BackendRunResult> RunBackendAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        // Safe no-op implementation for non-Windows platforms
        await Task.Delay(100, cancellationToken); // Brief delay to simulate work

        // Return diagnostic result indicating this platform is not supported
        return new BackendRunResult
        {
            Success = false,
            ErrorMessage = "Backend execution is only supported on Windows platform",
            Courses = new List<Course>(),
            // Populate diagnostics to show "inactive on this platform"
            BackendDirResolved = Path.Combine(AppContext.BaseDirectory ?? "", "Backend"),
            ConfigFound = false, // Not applicable on non-Windows
            ConfigWritten = false, // Not applicable on non-Windows  
            ExeFound = false, // Not available on non-Windows
            ProcessStarted = false, // Not available on non-Windows
            ProcessExited = false, // Not available on non-Windows
            ExitCode = null, // Not available on non-Windows
            StdErrSummary = "Platform not supported", // Clear indication
            ScheduleJsonFound = false, // Not applicable on non-Windows
            ScheduleParsed = false // Not applicable on non-Windows
        };
    }
}