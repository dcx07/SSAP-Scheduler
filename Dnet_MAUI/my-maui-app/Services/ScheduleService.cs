using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MyMauiApp.Models;

namespace MyMauiApp.Services
{
    public class ScheduleService
    {
        public async Task<List<Course>> FetchTodayCoursesAsync(string username, string password, string backendDir)
        {
            var configPath = Path.Combine(backendDir, "config.json");
            var schedulePath = Path.Combine(backendDir, "schedule_grouped.json");

            var config = new { Username = username, Password = password };
            await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(config));

            // Execute the external process (e.g., main.py or main.exe)
            var pythonFile = Path.Combine(backendDir, "main.py");
            var exeFile = Path.Combine(backendDir, "main.exe");
            
            var processStartInfo = new System.Diagnostics.ProcessStartInfo();

            // Try to run Python script first, then exe
            if (File.Exists(pythonFile))
            {
                processStartInfo.FileName = "python";
                processStartInfo.Arguments = "main.py";
            }
            else if (File.Exists(exeFile))
            {
                processStartInfo.FileName = exeFile;
            }
            else
            {
                throw new FileNotFoundException("Neither main.py nor main.exe found in backend directory");
            }

            processStartInfo.WorkingDirectory = backendDir;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            using (var process = System.Diagnostics.Process.Start(processStartInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    if (process.ExitCode != 0)
                    {
                        var error = await process.StandardError.ReadToEndAsync();
                        throw new Exception($"Backend execution error: {error}");
                    }
                }
            }

            if (!File.Exists(schedulePath))
            {
                throw new FileNotFoundException("Schedule file not found");
            }

            var jsonString = await File.ReadAllTextAsync(schedulePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);

            var todayKey = DateTime.Now.ToString("ddd", System.Globalization.CultureInfo.InvariantCulture);
            if (data != null && data.TryGetValue(todayKey, out var todayData) && todayData is JsonElement todayElement)
            {
                var courses = todayElement.GetProperty("courses").EnumerateArray();
                var courseList = new List<Course>();

                foreach (var course in courses)
                {
                    courseList.Add(new Course
                    {
                        Name = course.GetProperty("name").GetString() ?? "",
                        Start = course.GetProperty("start").GetString() ?? "",
                        End = course.GetProperty("end").GetString() ?? "",
                        Room = course.GetProperty("room").GetString() ?? "",
                        Teacher = course.GetProperty("teacher").GetString() ?? "",
                        Emoji = course.TryGetProperty("emoji", out var emojiProp) ? emojiProp.GetString() ?? "📚" : "📚"
                    });
                }

                return courseList;
            }

            return new List<Course>();
        }
    }
}