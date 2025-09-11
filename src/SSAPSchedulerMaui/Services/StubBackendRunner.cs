using SSAPSchedulerMaui.Models;

namespace SSAPSchedulerMaui.Services;

public class StubBackendRunner : IBackendRunner
{
    public async Task<BackendRunResult> RunBackendAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        // Simulate some work
        await Task.Delay(1000, cancellationToken);

        // Return stub data for non-Windows platforms
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
            Courses = stubCourses
        };
    }
}