using SSAPSchedulerMaui.Models;

namespace SSAPSchedulerMaui.Services;

public class StubBackendRunner : IBackendRunner
{
    public async Task<BackendRunResult> RunBackendAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        // Simulate some work
        await Task.Delay(1000, cancellationToken);

        // Return stub data for non-Windows platforms with appropriate diagnostics
        var stubCourses = new List<Course>
        {
            new Course
            {
                Name = "示例课程",
                Start = "09:00",
                End = "10:30",
                Location = "教室A",
                Emoji = "📚"
            },
            new Course
            {
                Name = "测试课程",
                Start = "14:00",
                End = "15:30",
                Location = "教室B",
                Emoji = "🔬"
            }
        };

        return new BackendRunResult
        {
            Success = true,
            Courses = stubCourses,
            // Populate diagnostics for stub implementation
            BackendDirResolved = Path.Combine(AppContext.BaseDirectory, "Backend"),
            ConfigWritten = true, // Simulated
            ExeFound = false, // Not available on non-Windows
            ProcessStarted = false, // Not available on non-Windows
            ProcessExited = false, // Not available on non-Windows
            ExitCode = null, // Not available on non-Windows
            ScheduleJsonFound = true, // Simulated
            ScheduleParsed = true // Simulated
        };
    }
}