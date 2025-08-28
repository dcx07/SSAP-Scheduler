using Microsoft.UI.Xaml;

namespace MyMauiApp.Platforms.Windows;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}