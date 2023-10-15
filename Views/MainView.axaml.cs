using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using BlackWater.Controls;
using BlackWater.ViewModels;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace BlackWater.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        GaugeChartControl c = (GaugeChartControl?)sender ?? new GaugeChartControl();
        new TaskFactory().StartNew(() => ChangeDataOnTimer(c, new CancellationToken()),
            TaskCreationOptions.LongRunning);
    }

    private async Task ChangeDataOnTimer(GaugeChartControl control, CancellationToken token = default)
    {
        Random rnd = new Random();
        while (!token.IsCancellationRequested)
        {
            var nextValue = rnd.Next(0, 110);
            control.NeedleValue = nextValue;
            await Task.Delay(1000, token);
        }
    }
}