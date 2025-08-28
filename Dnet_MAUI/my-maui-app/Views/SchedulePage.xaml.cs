using System;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyMauiApp.Views
{
    public partial class SchedulePage : ContentPage
    {
        private readonly ScheduleViewModel _viewModel;

        public SchedulePage(ScheduleViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadScheduleAsync();
        }

        private async Task LoadScheduleAsync()
        {
            await _viewModel.FetchTodayCoursesAsync();
        }

        private void OnRefreshButtonClicked(object sender, EventArgs e)
        {
            LoadScheduleAsync();
        }
    }
}