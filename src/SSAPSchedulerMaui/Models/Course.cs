namespace SSAPSchedulerMaui.Models;

public class Course
{
    public string Name { get; set; } = string.Empty;
    public string Start { get; set; } = string.Empty;
    public string End { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Emoji { get; set; } = "📚";
}

public class ScheduleData
{
    public Dictionary<string, DaySchedule> Days { get; set; } = new();
}

public class DaySchedule
{
    public List<Course> Courses { get; set; } = new();
}

public class LoginCredentials
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}