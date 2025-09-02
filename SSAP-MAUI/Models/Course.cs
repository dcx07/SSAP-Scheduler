using System.Text.Json.Serialization;

namespace SSAP_MAUI.Models;

public class Course
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("room")]
    public string Room { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public string Start { get; set; } = string.Empty;

    [JsonPropertyName("end")]
    public string End { get; set; } = string.Empty;

    [JsonPropertyName("teacher")]
    public string Teacher { get; set; } = string.Empty;

    [JsonPropertyName("emoji")]
    public string? Emoji { get; set; }

    // Computed property for display
    public string DisplayEmoji => Emoji ?? "📚";
    public string TimeRange => $"{Start} - {End}";
}

public class DaySchedule
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("courses")]
    public List<Course> Courses { get; set; } = new();
}

public class WeekSchedule
{
    [JsonPropertyName("Mon")]
    public DaySchedule? Monday { get; set; }

    [JsonPropertyName("Tue")]
    public DaySchedule? Tuesday { get; set; }

    [JsonPropertyName("Wed")]
    public DaySchedule? Wednesday { get; set; }

    [JsonPropertyName("Thu")]
    public DaySchedule? Thursday { get; set; }

    [JsonPropertyName("Fri")]
    public DaySchedule? Friday { get; set; }

    [JsonPropertyName("Sat")]
    public DaySchedule? Saturday { get; set; }

    [JsonPropertyName("Sun")]
    public DaySchedule? Sunday { get; set; }

    public DaySchedule? GetDaySchedule(DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        DayOfWeek.Monday => Monday,
        DayOfWeek.Tuesday => Tuesday,
        DayOfWeek.Wednesday => Wednesday,
        DayOfWeek.Thursday => Thursday,
        DayOfWeek.Friday => Friday,
        DayOfWeek.Saturday => Saturday,
        DayOfWeek.Sunday => Sunday,
        _ => null
    };
}

public class LoginConfig
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}