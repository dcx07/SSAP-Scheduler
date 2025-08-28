using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MyMauiApp.Models;
using MyMauiApp.Services;

namespace MyMauiApp.ViewModels
{
    public class ScheduleViewModel : ObservableObject
    {
        private readonly ScheduleService _scheduleService;
        private ObservableCollection<Course> _courses;
        private bool _isLoading;

        public ScheduleViewModel()
        {
            _scheduleService = new ScheduleService();
            Courses = new ObservableCollection<Course>();
            FetchScheduleCommand = new RelayCommand(async () => await FetchSchedule());
            IsLoading = false;
        }

        public ObservableCollection<Course> Courses
        {
            get => _courses;
            set => SetProperty(ref _courses, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand FetchScheduleCommand { get; }

        private async Task FetchSchedule()
        {
            IsLoading = true;
            try
            {
                var todayCourses = await _scheduleService.GetTodayCoursesAsync();
                Courses.Clear();
                foreach (var course in todayCourses)
                {
                    Courses.Add(course);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., show a message to the user)
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}