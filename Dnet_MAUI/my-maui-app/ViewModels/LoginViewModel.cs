using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using MyMauiApp.Services;

namespace MyMauiApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username = "";
        private string _password = "";
        private bool _isLoading;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await LoginAsync());
        }

        public async Task LoginAsync()
        {
            IsLoading = true;
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    await Application.Current?.MainPage?.DisplayAlert("错误", "请输入用户名和密码", "确定");
                    return;
                }

                // Navigate to schedule page
                await Shell.Current.GoToAsync($"//SchedulePage?username={Uri.EscapeDataString(Username)}&password={Uri.EscapeDataString(Password)}");
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("错误", $"登录失败: {ex.Message}", "确定");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}