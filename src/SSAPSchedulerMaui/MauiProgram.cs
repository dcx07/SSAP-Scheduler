using Microsoft.Extensions.Logging;
using SSAPSchedulerMaui.Services;
using SSAPSchedulerMaui.Views;
using SSAPSchedulerMaui.ViewModels;

namespace SSAPSchedulerMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				// Add Chinese fonts
				fonts.AddFont("NotoSansSC-Regular.ttf", "NotoSansSC");
				fonts.AddFont("NotoSansSC-Bold.ttf", "NotoSansSCBold");
			});

		// Register services
		builder.Services.AddSingleton<IBackendService, BackendService>();
		
		// Register platform-specific backend runner
#if WINDOWS
		builder.Services.AddSingleton<IBackendRunner, WindowsBackendRunner>();
#else
		builder.Services.AddSingleton<IBackendRunner, StubBackendRunner>();
#endif
		
		// Register ViewModels
		builder.Services.AddTransient<LoginPageViewModel>();
		
		// Register pages
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<SchedulePage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
