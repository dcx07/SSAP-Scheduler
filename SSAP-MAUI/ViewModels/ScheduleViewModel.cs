using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSAP_MAUI.Models;
using SSAP_MAUI.Services;
using Microsoft.Extensions.Logging;

namespace SSAP_MAUI.ViewModels;

[QueryProperty(nameof(Username), "username")]
[QueryProperty(nameof(Password), "password")]
public partial class ScheduleViewModel : ObservableObject
{
    private readonly IScheduleService _scheduleService;
    private readonly ILogger<ScheduleViewModel> _logger;

    public ScheduleViewModel(IScheduleService scheduleService, ILogger<ScheduleViewModel> logger)
    {
        _scheduleService = scheduleService;
        _logger = logger;
        Courses = new ObservableCollection<Course>();
        
        // Set Chinese culture for date formatting
        CultureInfo.CurrentCulture = new CultureInfo("zh-CN");
        CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
    }

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isLoading = true;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string todayDate = string.Empty;

    public ObservableCollection<Course> Courses { get; }

    partial void OnUsernameChanged(string value)
    {
        UpdateTodayDate();
    }

    public async Task InitializeAsync()
    {
        UpdateTodayDate();
        await RefreshScheduleAsync();
    }

    [RelayCommand]
    public async Task RefreshScheduleAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            _logger.LogInformation("Refreshing schedule...");

            // Save config again in case it's needed
            await _scheduleService.SaveConfigAsync(Username, Password);

            // Run backend process
            var backendResult = await _scheduleService.RunBackendAsync();
            if (!backendResult)
            {
                ErrorMessage = "后端处理失败，请检查配置和网络连接";
                return;
            }

            // Load today's schedule
            var todayCourses = await _scheduleService.GetTodayScheduleAsync();

            // Update UI
            Courses.Clear();
            foreach (var course in todayCourses)
            {
                Courses.Add(course);
            }

            if (Courses.Count == 0)
            {
                ErrorMessage = "今天没有课程安排";
            }

            _logger.LogInformation($"Loaded {Courses.Count} courses for today");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"更新日程失败: {ex.Message}";
            _logger.LogError(ex, "Failed to refresh schedule");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateTodayDate()
    {
        try
        {
            var now = DateTime.Now;
            var culture = new CultureInfo("zh-CN");
            TodayDate = now.ToString("yyyy年M月d日 dddd", culture);
        }
        catch
        {
            TodayDate = DateTime.Now.ToString("yyyy-MM-dd dddd");
        }
    }
}