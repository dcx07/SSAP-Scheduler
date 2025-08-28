using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyMauiApp.Services
{
    public class ScheduleService
    {
        private readonly string _backendDir;

        public ScheduleService(string backendDir)
        {
            _backendDir = backendDir;
        }

        public async Task<List<Course>> FetchTodayCoursesAsync(string username, string password)
        {
            var configPath = Path.Combine(_backendDir, "config.json");
            var schedulePath = Path.Combine(_backendDir, "schedule_grouped.json");

            var config = new { Username = username, Password = password };
            await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(config));

            // Execute the external process (e.g., an EXE file)
            var exeFile = "main.exe";
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = exeFile,
                WorkingDirectory = _backendDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = System.Diagnostics.Process.Start(processStartInfo))
            {
                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"EXE execution error: {error}");
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
                        Name = course.GetProperty("name").GetString(),
                        Start = course.GetProperty("start").GetString(),
                        End = course.GetProperty("end").GetString(),
                        Room = course.GetProperty("room").GetString(),
                        Teacher = course.GetProperty("teacher").GetString(),
                        Emoji = course.GetProperty("emoji").GetString()
                    });
                }

                return courseList;
            }

            return new List<Course>();
        }
    }
}