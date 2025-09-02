using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSAP_MAUI.Services;
using SSAP_MAUI.Views;
using Microsoft.Extensions.Logging;

namespace SSAP_MAUI.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IScheduleService _scheduleService;
    private readonly ILogger<LoginViewModel> _logger;

    public LoginViewModel(IScheduleService scheduleService, ILogger<LoginViewModel> logger)
    {
        _scheduleService = scheduleService;
        _logger = logger;
    }

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (IsLoading) return;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "请输入用户名和密码";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            _logger.LogInformation($"Attempting login for user: {Username}");
            
            // Save configuration
            var configSaved = await _scheduleService.SaveConfigAsync(Username, Password);
            
            if (!configSaved)
            {
                ErrorMessage = "保存配置失败";
                return;
            }

            // Navigate to schedule page
            await Shell.Current.GoToAsync($"{nameof(SchedulePage)}?username={Uri.EscapeDataString(Username)}&password={Uri.EscapeDataString(Password)}");
            
            _logger.LogInformation("Login successful");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"登录失败: {ex.Message}";
            _logger.LogError(ex, "Login failed");
        }
        finally
        {
            IsLoading = false;
        }
    }
}