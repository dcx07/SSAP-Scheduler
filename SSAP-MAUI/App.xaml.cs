using SSAP_MAUI.Views;

namespace SSAP_MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        
        // Set window properties for better experience
        window.Title = "SSAP 课程调度器";
        
        const int newWidth = 800;
        const int newHeight = 600;

        window.Width = newWidth;
        window.Height = newHeight;
        
        return window;
    }
}