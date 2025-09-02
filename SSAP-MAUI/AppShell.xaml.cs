using SSAP_MAUI.Views;

namespace SSAP_MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(SchedulePage), typeof(SchedulePage));
    }
}