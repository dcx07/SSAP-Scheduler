using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace my_maui_app.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
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
            LoginCommand = new Command(async () => await Login());
        }

        private async Task Login()
        {
            IsLoading = true;
            try
            {
                // Implement login logic here, such as calling a service to authenticate
                // For example:
                // var result = await AuthService.Login(Username, Password);
                // if (result.IsSuccess)
                // {
                //     // Navigate to the schedule page
                // }
            }
            catch (Exception ex)
            {
                // Handle login failure (e.g., show a message)
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