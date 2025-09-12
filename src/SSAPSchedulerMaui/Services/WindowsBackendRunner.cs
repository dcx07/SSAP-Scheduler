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
            // Platform guard - only execute on Windows
            if (!OperatingSystem.IsWindows())
            {
                result.ErrorMessage = "Backend execution is only supported on Windows platform";
                result.Success = false;
                return result;
            }

            // Step 1: Resolve and validate backend directory
            var backendDir = GetBackendDirectory();
            result.BackendDirResolved = backendDir;
            
            if (!Directory.Exists(backendDir))
            {
                result.ErrorMessage = $"Backend directory not found: {backendDir}";
                result.Success = false;
                return result;
            }

            // Step 2: Validate main.exe exists (in dist subdirectory)
            var exePath = Path.Combine(backendDir, "dist", "main.exe");
            result.ExeFound = File.Exists(exePath);
            
            if (!result.ExeFound)
            {
                result.ErrorMessage = $"Backend executable not found: {exePath}";
                result.Success = false;
                return result;
            }

            // Step 3: Write credentials to config.json
            var configPath = Path.Combine(backendDir, "config.json");
            try
            {
                await WriteCredentialsAsync(backendDir, username, password);
                result.ConfigWritten = true;
                result.ConfigFound = File.Exists(configPath);
            }
            catch (Exception ex)
            {
                result.ConfigWritten = false;
                result.ConfigFound = false;
                result.ErrorMessage = $"Failed to write config.json: {ex.Message}";
                result.Success = false;
                return result;
            }

            // Step 4: Execute main.exe and wait for completion
            string stdErr = string.Empty;
            try
            {
                var (exitCode, processStarted, processExited, capturedStdErr) = await ExecuteBackendAsync(backendDir, cancellationToken);
                result.ProcessStarted = processStarted;
                result.ProcessExited = processExited;
                result.ExitCode = exitCode;
                stdErr = capturedStdErr;
                
                // Provide summary of stderr (first 200 chars to avoid exposing sensitive data)
                result.StdErrSummary = string.IsNullOrEmpty(stdErr) ? string.Empty : 
                    stdErr.Length > 200 ? stdErr.Substring(0, 200) + "..." : stdErr;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Process execution failed: {ex.Message}";
                result.Success = false;
                return result;
            }

            // Step 5: Read and parse schedule_grouped.json
            var schedulePath = Path.Combine(backendDir, "schedule_grouped.json");
            result.ScheduleJsonFound = File.Exists(schedulePath);
            
            if (!result.ScheduleJsonFound)
            {
                result.ErrorMessage = $"Schedule file not found after backend execution: {schedulePath}";
                result.Success = false;
                return result;
            }

            try
            {
                var courses = await ReadScheduleAsync(backendDir);
                result.ScheduleParsed = true;
                result.Courses = courses;
            }
            catch (Exception ex)
            {
                result.ScheduleParsed = false;
                result.ErrorMessage = $"Failed to parse schedule data: {ex.Message}";
                result.Success = false;
                return result;
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
        // Use AppContext.BaseDirectory and Path.Combine as specified in requirements
        var baseDirectory = AppContext.BaseDirectory;
        if (string.IsNullOrEmpty(baseDirectory))
        {
            throw new InvalidOperationException("Unable to resolve application base directory");
        }

        var backendDir = Path.Combine(baseDirectory, "Backend");
        
        // Validate that backend directory exists
        if (!Directory.Exists(backendDir))
        {
            throw new DirectoryNotFoundException($"Backend directory not found at: {backendDir}");
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
        await File.WriteAllTextAsync(configPath, jsonString, System.Text.Encoding.UTF8);
        
        // Validate that config.json was written successfully
        if (!File.Exists(configPath))
        {
            throw new InvalidOperationException($"Failed to create config.json at: {configPath}");
        }
    }

    private async Task<(int exitCode, bool processStarted, bool processExited, string stdErr)> ExecuteBackendAsync(string backendDir, CancellationToken cancellationToken)
    {
        var exePath = Path.Combine(backendDir, "dist", "main.exe");
        
        // Validate existence with explicit error message
        if (!File.Exists(exePath))
        {
            throw new FileNotFoundException($"Backend executable not found at: {exePath}");
        }

        // Validate config.json exists
        var configPath = Path.Combine(backendDir, "config.json");
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found at: {configPath}");
        }

        var processInfo = new ProcessStartInfo
        {
            FileName = exePath, // Fully qualified path
            WorkingDirectory = backendDir, // Set working directory to Backend (not dist)
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };
        
        if (!process.Start())
        {
            throw new InvalidOperationException("Failed to start backend process");
        }
        
        var processStarted = true;
        
        // Read output streams asynchronously
        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        
        // Wait for completion with timeout and cancellation support
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
            try
            {
                process.Kill();
            }
            catch
            {
                // Ignore errors when killing process
            }
            
            if (timeoutCts.Token.IsCancellationRequested)
            {
                throw new TimeoutException($"Backend process timed out after {TIMEOUT_SECONDS} seconds");
            }
            throw;
        }
        
        // Get output and error streams
        var output = await outputTask;
        var error = await errorTask;
        
        // Check exit code and provide descriptive error
        if (process.ExitCode != 0)
        {
            var errorMsg = string.IsNullOrEmpty(error) ? 
                $"Process exited with code {process.ExitCode}" : 
                $"Process failed with exit code {process.ExitCode}: {error}";
            throw new InvalidOperationException(errorMsg);
        }
        
        return (process.ExitCode, processStarted, processExited, error);
    }

    private async Task<List<Course>> ReadScheduleAsync(string backendDir)
    {
        var schedulePath = Path.Combine(backendDir, "schedule_grouped.json");
        
        if (!File.Exists(schedulePath))
        {
            throw new FileNotFoundException($"Schedule file not found at: {schedulePath}");
        }

        try
        {
            var jsonContent = await File.ReadAllTextAsync(schedulePath, System.Text.Encoding.UTF8);
            return ParseTodaysCourses(jsonContent);
        }
        catch (Exception ex) when (!(ex is FileNotFoundException))
        {
            throw new InvalidOperationException($"Failed to read schedule file from {schedulePath}: {ex.Message}", ex);
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
}