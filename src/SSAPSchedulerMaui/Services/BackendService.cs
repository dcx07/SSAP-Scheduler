using SSAPSchedulerMaui.Models;
using System.Text.Json;
using System.Diagnostics;

namespace SSAPSchedulerMaui.Services;

public class BackendService : IBackendService
{
    public async Task<string> GetBackendDirectoryAsync()
    {
        // Get the application's base directory and navigate to Backend folder
        // This mimics the Flutter app's getBackendDir() function
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        
        // Navigate up from the executable location to find the project root
        var projectRoot = Directory.GetParent(baseDirectory);
        while (projectRoot != null && !Directory.Exists(Path.Combine(projectRoot.FullName, "Backend")))
        {
            projectRoot = projectRoot.Parent;
        }
        
        if (projectRoot == null)
        {
            throw new DirectoryNotFoundException("Could not find project root with Backend directory");
        }
        
        var backendDir = Path.Combine(projectRoot.FullName, "Backend");
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(backendDir))
        {
            Directory.CreateDirectory(backendDir);
        }
        
        return backendDir;
    }

    public async Task SaveCredentialsAsync(string username, string password)
    {
        try
        {
            var backendDir = await GetBackendDirectoryAsync();
            var configPath = Path.Combine(backendDir, "config.json");
            
            var credentials = new LoginCredentials
            {
                Username = username,
                Password = password
            };
            
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            
            var jsonString = JsonSerializer.Serialize(credentials, jsonOptions);
            await File.WriteAllTextAsync(configPath, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save credentials: {ex.Message}", ex);
        }
    }

    public async Task<List<Course>> GetTodayCoursesAsync()
    {
        try
        {
            var backendDir = await GetBackendDirectoryAsync();
            var schedulePath = Path.Combine(backendDir, "schedule_grouped.json");
            var exePath = Path.Combine(backendDir, "main.exe");

            // Execute the backend executable (Windows-specific)
#if WINDOWS
            if (File.Exists(exePath))
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = backendDir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();
                
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                
                await process.WaitForExitAsync();
                
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Backend execution failed: {error}");
                }
            }
            else
            {
                // TODO: Handle case when main.exe doesn't exist
                throw new FileNotFoundException("Backend executable not found");
            }
#else
            // For non-Windows platforms, create stub data
            await CreateStubScheduleDataAsync(schedulePath);
#endif

            // Read and parse the schedule data
            if (File.Exists(schedulePath))
            {
                var jsonContent = await File.ReadAllTextAsync(schedulePath);
                return ParseTodaysCourses(jsonContent);
            }
            else
            {
                throw new FileNotFoundException("Schedule file not found after backend execution");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get today's courses: {ex.Message}", ex);
        }
    }

    private List<Course> ParseTodaysCourses(string jsonContent)
    {
        try
        {
            var scheduleData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonContent);
            
            // Get today's weekday (Mon, Tue, Wed, Thu, Fri, Sat, Sun)
            var weekdays = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            var todayKey = weekdays[(int)DateTime.Now.DayOfWeek == 0 ? 6 : (int)DateTime.Now.DayOfWeek - 1];
            
            if (scheduleData != null && scheduleData.TryGetValue(todayKey, out var todayData))
            {
                if (todayData.TryGetProperty("courses", out var coursesElement) && coursesElement.ValueKind == JsonValueKind.Array)
                {
                    var courses = new List<Course>();
                    
                    foreach (var courseElement in coursesElement.EnumerateArray())
                    {
                        var course = new Course();
                        
                        if (courseElement.TryGetProperty("name", out var nameElement))
                            course.Name = nameElement.GetString() ?? "";
                            
                        if (courseElement.TryGetProperty("start", out var startElement))
                            course.Start = startElement.GetString() ?? "";
                            
                        if (courseElement.TryGetProperty("end", out var endElement))
                            course.End = endElement.GetString() ?? "";
                            
                        if (courseElement.TryGetProperty("location", out var locationElement))
                            course.Location = locationElement.GetString() ?? "";
                            
                        if (courseElement.TryGetProperty("emoji", out var emojiElement))
                            course.Emoji = emojiElement.GetString() ?? "📚";
                        
                        courses.Add(course);
                    }
                    
                    // Reverse the list to match Flutter behavior
                    courses.Reverse();
                    return courses;
                }
            }
            
            return new List<Course>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse schedule data: {ex.Message}", ex);
        }
    }

    private async Task CreateStubScheduleDataAsync(string schedulePath)
    {
        // Create stub data for development/testing purposes
        var stubData = new Dictionary<string, object>
        {
            ["Mon"] = new { courses = new[]
                {
                    new { name = "示例课程", start = "09:00", end = "10:30", location = "教室A", emoji = "📚" },
                    new { name = "测试课程", start = "14:00", end = "15:30", location = "教室B", emoji = "🔬" }
                }
            },
            ["Tue"] = new { courses = new object[] { } },
            ["Wed"] = new { courses = new object[] { } },
            ["Thu"] = new { courses = new object[] { } },
            ["Fri"] = new { courses = new object[] { } },
            ["Sat"] = new { courses = new object[] { } },
            ["Sun"] = new { courses = new object[] { } }
        };

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var jsonString = JsonSerializer.Serialize(stubData, jsonOptions);
        await File.WriteAllTextAsync(schedulePath, jsonString);
    }
}