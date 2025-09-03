using Microsoft.Extensions.Logging;
using SSAPSchedulerMaui.Services;
using SSAPSchedulerMaui.Views;

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
		
		// Register pages
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<SchedulePage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
