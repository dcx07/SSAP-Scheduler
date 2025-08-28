using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp.ViewModels
{
    public partial class ScheduleViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ScheduleService _scheduleService;

        [ObservableProperty]
        private ObservableCollection<Course> _courses;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _password = "";

        public ScheduleViewModel(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
            Courses = new ObservableCollection<Course>();
            IsLoading = false;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("username", out var username))
                Username = Uri.UnescapeDataString(username.ToString() ?? "");
            
            if (query.TryGetValue("password", out var password))
                Password = Uri.UnescapeDataString(password.ToString() ?? "");

            // Load schedule when navigating to this page
            _ = FetchScheduleCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task FetchSchedule()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                return;

            IsLoading = true;
            try
            {
                // Get the Backend directory - assuming it's in the parent directory of the app
                var backendDir = Path.Combine(FileSystem.AppDataDirectory, "..", "..", "Backend");
                var todayCourses = await _scheduleService.FetchTodayCoursesAsync(Username, Password, backendDir);
                
                Courses.Clear();
                foreach (var course in todayCourses)
                {
                    Courses.Add(course);
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("错误", $"获取课程表失败: {ex.Message}", "确定");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}