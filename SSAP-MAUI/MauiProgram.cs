using Microsoft.Extensions.Logging;
using SSAP_MAUI.Services;
using SSAP_MAUI.ViewModels;
using SSAP_MAUI.Views;

namespace SSAP_MAUI;

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
                fonts.AddFont("NotoSansSC-Regular.ttf", "NotoSansSC");
            });

        // Register services
        builder.Services.AddSingleton<IScheduleService, ScheduleService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ScheduleViewModel>();

        // Register Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SchedulePage>();

        builder.Services.AddLogging(configure => configure.AddDebug());

        return builder.Build();
    }
}