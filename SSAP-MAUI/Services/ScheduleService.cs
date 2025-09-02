using System.Diagnostics;
using System.Text.Json;
using SSAP_MAUI.Models;
using Microsoft.Extensions.Logging;

namespace SSAP_MAUI.Services;

public class ScheduleService : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger;

    public ScheduleService(ILogger<ScheduleService> logger)
    {
        _logger = logger;
    }

    public string GetBackendDirectory()
    {
        try 
        {
            // Get the application directory
            var appDir = AppContext.BaseDirectory;
            
            // Navigate to the project root and find Backend directory
            var projectRoot = FindProjectRoot(appDir);
            if (projectRoot != null)
            {
                var backendDir = Path.Combine(projectRoot, "Backend");
                if (Directory.Exists(backendDir))
                {
                    _logger.LogInformation($"Found Backend directory at: {backendDir}");
                    return backendDir;
                }
            }

            // Fallback: try relative to application directory
            var fallbackDir = Path.Combine(appDir, "..", "..", "..", "..", "Backend");
            var normalizedFallback = Path.GetFullPath(fallbackDir);
            
            if (Directory.Exists(normalizedFallback))
            {
                _logger.LogInformation($"Using fallback Backend directory: {normalizedFallback}");
                return normalizedFallback;
            }

            // Last resort: create Backend directory in app directory
            var lastResortDir = Path.Combine(appDir, "Backend");
            Directory.CreateDirectory(lastResortDir);
            _logger.LogWarning($"Created new Backend directory: {lastResortDir}");
            return lastResortDir;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Backend directory");
            throw;
        }
    }

    private string? FindProjectRoot(string startPath)
    {
        var current = new DirectoryInfo(startPath);
        while (current != null)
        {
            // Look for project indicators
            if (current.GetFiles("*.sln").Length > 0 || 
                current.GetDirectories("Backend").Length > 0 ||
                current.GetFiles("*.csproj").Any(f => f.Name.Contains("SSAP")))
            {
                return current.FullName;
            }
            current = current.Parent;
        }
        return null;
    }

    public async Task<bool> SaveConfigAsync(string username, string password)
    {
        try
        {
            var config = new LoginConfig
            {
                Username = username,
                Password = password
            };

            var backendDir = GetBackendDirectory();
            var configPath = Path.Combine(backendDir, "config.json");
            
            var jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            
            await File.WriteAllTextAsync(configPath, jsonString);
            _logger.LogInformation($"Config saved to: {configPath}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving config");
            return false;
        }
    }

    public async Task<bool> RunBackendAsync()
    {
        try
        {
            var backendDir = GetBackendDirectory();
            var exePath = Path.Combine(backendDir, "main.exe");

            if (!File.Exists(exePath))
            {
                _logger.LogError($"Backend executable not found at: {exePath}");
                return false;
            }

            using var process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.WorkingDirectory = backendDir;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            var output = new List<string>();
            var errors = new List<string>();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    output.Add(e.Data);
                    _logger.LogDebug($"Backend output: {e.Data}");
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errors.Add(e.Data);
                    _logger.LogWarning($"Backend error: {e.Data}");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for process to complete with timeout
            var completed = await process.WaitForExitAsync();
            
            if (process.ExitCode == 0)
            {
                _logger.LogInformation("Backend process completed successfully");
                return true;
            }
            else
            {
                _logger.LogError($"Backend process failed with exit code: {process.ExitCode}");
                _logger.LogError($"Errors: {string.Join(Environment.NewLine, errors)}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running backend process");
            return false;
        }
    }

    public async Task<List<Course>> GetTodayScheduleAsync()
    {
        try
        {
            var backendDir = GetBackendDirectory();
            var schedulePath = Path.Combine(backendDir, "schedule_grouped.json");

            if (!File.Exists(schedulePath))
            {
                _logger.LogWarning($"Schedule file not found at: {schedulePath}");
                return new List<Course>();
            }

            var jsonString = await File.ReadAllTextAsync(schedulePath);
            var weekSchedule = JsonSerializer.Deserialize<WeekSchedule>(jsonString);

            if (weekSchedule == null)
            {
                _logger.LogWarning("Failed to deserialize schedule data");
                return new List<Course>();
            }

            var today = DateTime.Now.DayOfWeek;
            var todaySchedule = weekSchedule.GetDaySchedule(today);

            if (todaySchedule?.Courses == null)
            {
                _logger.LogInformation($"No courses found for {today}");
                return new List<Course>();
            }

            // Return courses in reverse order to match Flutter behavior
            var courses = todaySchedule.Courses.ToList();
            courses.Reverse();

            _logger.LogInformation($"Found {courses.Count} courses for {today}");
            return courses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading schedule");
            return new List<Course>();
        }
    }
}