using Microsoft.Extensions.Logging;
using MyMauiApp.ViewModels;
using MyMauiApp.Views;
using MyMauiApp.Services;

namespace MyMauiApp;

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
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        // Register services
        builder.Services.AddSingleton<ScheduleService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ScheduleViewModel>();

        // Register Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SchedulePage>();

		return builder.Build();
	}
}