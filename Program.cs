using Avalonia;
using Avalonia.LinuxFramebuffer.Output;
using System;
using System.Linq;

namespace BlackWater;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        
        var builder = BuildAvaloniaApp();
        if (args.Contains("--drm"))
        {
            return builder.StartLinuxDrm(args, null, 1);
        }
        if (args.Contains("--fb"))
        {
            return builder.StartLinuxFbDev(args);
        }
        if (args.Contains("--direct"))
        {
            return builder.StartLinuxDirect(args, new DrmOutput());
        }

        return builder.StartWithClassicDesktopLifetime(args);
    }
       

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
