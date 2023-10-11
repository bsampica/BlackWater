using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BlackWater.ViewModels;
using BlackWater.Views;
using MQTTnet.Internal;

namespace BlackWater;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            singleView.MainView = new MainView();

        base.OnFrameworkInitializationCompleted();

        var tc = new TelemetryConnection();

        Task.Run(async () =>
        {
            await tc.ConnectAsync().ContinueWith((s) => tc.PublishLoopAsync());
            // await tc.PublishLoopAsync();
        });
        
    }
}