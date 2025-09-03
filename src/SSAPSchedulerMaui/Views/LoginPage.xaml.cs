using SSAPSchedulerMaui.Services;
using SSAPSchedulerMaui.Views;
using System.Text.Json;

namespace SSAPSchedulerMaui.Views;

public partial class LoginPage : ContentPage
{
    private readonly IBackendService _backendService;

    public LoginPage(IBackendService backendService)
    {
        InitializeComponent();
        _backendService = backendService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("错误", "请输入用户名和密码", "确定");
            return;
        }

        // Show loading state
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        LoginButton.IsEnabled = false;

        try
        {
            // Save credentials to config.json
            await _backendService.SaveCredentialsAsync(UsernameEntry.Text, PasswordEntry.Text);

            // Navigate to Schedule page
            await Shell.Current.GoToAsync($"///{nameof(SchedulePage)}", new Dictionary<string, object>
            {
                { "Username", UsernameEntry.Text },
                { "Password", PasswordEntry.Text }
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("登录失败", $"登录过程中发生错误: {ex.Message}", "确定");
        }
        finally
        {
            // Hide loading state
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            LoginButton.IsEnabled = true;
        }
    }
}