using System;
using Microsoft.Maui.Controls;
using my_maui_app.ViewModels;

namespace my_maui_app.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            BindingContext = _viewModel;
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (_viewModel.IsLoading)
                return;

            await _viewModel.LoginAsync();
        }
    }
}