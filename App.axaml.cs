using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BlackWater.Views;
using BlackWater.Mqtt;

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
        Task.Run(() =>
        {
            tc.ConnectAsync().ContinueWith(async (task) =>
            {
                await tc.SubscribeAsync();
                await tc.PublishLoopAsync();
            });
        
            // await tc.PublishLoopAsync();
        });
    }
}