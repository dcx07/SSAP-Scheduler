using SSAPSchedulerMaui.Views;

namespace SSAPSchedulerMaui;

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
