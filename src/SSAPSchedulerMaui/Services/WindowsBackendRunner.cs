using SSAPSchedulerMaui.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SSAPSchedulerMaui.Services;

public class WindowsBackendRunner : IBackendRunner
{
    private const int TIMEOUT_SECONDS = 30;

    public async Task<BackendRunResult> RunBackendAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var result = new BackendRunResult();
        
        try
        {
            // Step 1: Write credentials to config.json
            var backendDir = GetBackendDirectory();
            result.BackendDirResolved = backendDir;
            
            try
            {
                await WriteCredentialsAsync(backendDir, username, password);
                result.ConfigWritten = true;
            }
            catch
            {
                result.ConfigWritten = false;
                throw;
            }

            // Check if exe exists before attempting to execute
            var exePath = Path.Combine(backendDir, "main.exe");
            result.ExeFound = File.Exists(exePath);

            // Step 2: Execute main.exe and wait for completion
            try
            {
                var (exitCode, processStarted, processExited) = await ExecuteBackendAsync(backendDir, cancellationToken);
                result.ProcessStarted = processStarted;
                result.ProcessExited = processExited;
                result.ExitCode = exitCode;
            }
            catch
            {
                // Process execution failed, but we should still check for diagnostics
                throw;
            }

            // Step 3: Read and parse schedule_grouped.json
            var schedulePath = Path.Combine(backendDir, "schedule_grouped.json");
            result.ScheduleJsonFound = File.Exists(schedulePath);
            
            try
            {
                var courses = await ReadScheduleAsync(backendDir);
                result.ScheduleParsed = true;
                result.Courses = courses;
            }
            catch
            {
                result.ScheduleParsed = false;
                throw;
            }
            
            result.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            // Redact password from error messages
            var errorMessage = ex.Message.Replace(password, "***");
            result.Success = false;
            result.ErrorMessage = errorMessage;
            return result;
        }
    }

    private string GetBackendDirectory()
    {
        // Use AppContext.BaseDirectory as specified in requirements
        var baseDirectory = AppContext.BaseDirectory;
        var backendDir = Path.Combine(baseDirectory, "Backend");

        if (!Directory.Exists(backendDir))
        {
            Directory.CreateDirectory(backendDir);
        }

        return backendDir;
    }

    private async Task WriteCredentialsAsync(string backendDir, string username, string password)
    {
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

    private async Task<(int exitCode, bool processStarted, bool processExited)> ExecuteBackendAsync(string backendDir, CancellationToken cancellationToken)
    {
        var exePath = Path.Combine(backendDir, "main.exe");
        
        if (!File.Exists(exePath))
        {
            throw new FileNotFoundException("Backend executable (main.exe) not found");
        }

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
        var processStarted = true;
        
        // Read output streams
        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        
        // Wait for completion with timeout and cancellation
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS));
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        
        bool processExited = false;
        try
        {
            await process.WaitForExitAsync(combinedCts.Token);
            processExited = true;
        }
        catch (OperationCanceledException)
        {
            process.Kill();
            if (timeoutCts.Token.IsCancellationRequested)
            {
                throw new TimeoutException($"Backend process timed out after {TIMEOUT_SECONDS} seconds");
            }
            throw;
        }
        
        var output = await outputTask;
        var error = await errorTask;
        
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Backend process failed with exit code {process.ExitCode}: {error}");
        }
        
        return (process.ExitCode, processStarted, processExited);
    }

    private async Task<List<Course>> ReadScheduleAsync(string backendDir)
    {
        var schedulePath = Path.Combine(backendDir, "schedule_grouped.json");
        
        if (!File.Exists(schedulePath))
        {
            throw new FileNotFoundException("Schedule file (schedule_grouped.json) not found after backend execution");
        }

        var jsonContent = await File.ReadAllTextAsync(schedulePath);
        return ParseTodaysCourses(jsonContent);
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
}