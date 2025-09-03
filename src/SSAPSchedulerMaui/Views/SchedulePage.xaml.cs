using SSAPSchedulerMaui.Models;
using SSAPSchedulerMaui.Services;
using System.Collections.ObjectModel;

namespace SSAPSchedulerMaui.Views;

[QueryProperty(nameof(Username), "Username")]
[QueryProperty(nameof(Password), "Password")]
public partial class SchedulePage : ContentPage
{
    private readonly IBackendService _backendService;
    private ObservableCollection<Course> _courses;

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ObservableCollection<Course> Courses
    {
        get => _courses;
        set
        {
            _courses = value;
            OnPropertyChanged();
        }
    }

    public SchedulePage(IBackendService backendService)
    {
        InitializeComponent();
        _backendService = backendService;
        _courses = new ObservableCollection<Course>();
        CoursesCollectionView.ItemsSource = _courses;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadScheduleAsync();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadScheduleAsync();
    }

    private async Task LoadScheduleAsync()
    {
        try
        {
            ShowLoading(true);

            // Save credentials and fetch schedule data
            await _backendService.SaveCredentialsAsync(Username, Password);
            var courses = await _backendService.GetTodayCoursesAsync();

            // Update UI on main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _courses.Clear();
                foreach (var course in courses)
                {
                    _courses.Add(course);
                }

                ShowLoading(false);
                UpdateEmptyState();
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ShowLoading(false);
                DisplayAlert("错误", $"更新日程失败: {ex.Message}", "确定");
                UpdateEmptyState();
            });
        }
    }

    private void ShowLoading(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        CoursesCollectionView.IsVisible = !isLoading;
        RefreshButton.IsEnabled = !isLoading;
    }

    private void UpdateEmptyState()
    {
        bool hasCourses = _courses.Count > 0;
        CoursesCollectionView.IsVisible = hasCourses;
        EmptyStateLayout.IsVisible = !hasCourses && !LoadingIndicator.IsVisible;
    }
}