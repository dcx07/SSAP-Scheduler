using SSAPSchedulerMaui.ViewModels;

namespace SSAPSchedulerMaui.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Cleanup cancellation tokens when page is disposed
        if (BindingContext is LoginPageViewModel viewModel)
        {
            viewModel.Cleanup();
        }
    }
}