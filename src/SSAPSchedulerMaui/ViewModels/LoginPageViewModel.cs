using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSAPSchedulerMaui.Services;
using SSAPSchedulerMaui.Views;

namespace SSAPSchedulerMaui.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    private readonly IBackendRunner _backendRunner;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    private CancellationTokenSource? _cancellationTokenSource;

    public LoginPageViewModel(IBackendRunner backendRunner)
    {
        _backendRunner = backendRunner;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "请输入用户名和密码";
            return;
        }

        if (IsLoading)
        {
            return; // Concurrency guard
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        IsLoading = true;
        StatusMessage = "正在登录...";

        try
        {
            // Execute the 1→2→3 flow: credentials → config.json → main.exe → schedule.json
            StatusMessage = "正在验证凭据...";
            var result = await _backendRunner.RunBackendAsync(Username, Password, _cancellationTokenSource.Token);

            if (result.Success)
            {
                StatusMessage = "登录成功";
                
                // Navigate to Schedule page with the loaded data
                await Shell.Current.GoToAsync($"///{nameof(SchedulePage)}", new Dictionary<string, object>
                {
                    { "Username", Username },
                    { "Password", Password },
                    { "PreloadedCourses", result.Courses }
                });
            }
            else
            {
                StatusMessage = $"登录失败: {result.ErrorMessage}";
            }
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "登录已取消";
        }
        catch (Exception ex)
        {
            // Redact password from error messages
            var errorMessage = ex.Message.Replace(Password, "***");
            StatusMessage = $"登录过程中发生错误: {errorMessage}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void CancelLogin()
    {
        _cancellationTokenSource?.Cancel();
        StatusMessage = "正在取消...";
    }

    public void Cleanup()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}