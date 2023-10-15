using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;

namespace BlackWater.Controls;

public partial class GaugeChartControl : UserControl
{
    public static readonly DirectProperty<GaugeChartControl, string> ChartTitleProperty =
        AvaloniaProperty.RegisterDirect<GaugeChartControl, string>(
            nameof(ChartTitle),
            o => o._chartTitle,
            (o, v) => o._chartTitle = v);

    private string _chartTitle = "";

    public string ChartTitle
    {
        get => _chartTitle;
        set => SetAndRaise(ChartTitleProperty, ref _chartTitle, value);
    }


    // public static DirectProperty<GaugeChartControl, string> ChartTitleProperty =
    //     AvaloniaProperty.RegisterDirect<GaugeChartControl, string>(nameof(ChartTitle),
    //         (o) => o.ChartTitle,
    //         ((control, s) => control.ChartTitle = s));

    // public static readonly AttachedProperty<string> ChartTitleProperty =
    //     AvaloniaProperty.RegisterAttached<GaugeChartControl, string>(nameof(ChartTitle), typeof(GaugeChartControl));

    // public static readonly StyledProperty<string> ChartTitleProperty =
    //     AvaloniaProperty.Register<GaugeChartControl, string>(nameof(ChartTitle));
    // public string ChartTitle
    // {
    //     get => GetValue(ChartTitleProperty);
    //     set => SetValue(ChartTitleProperty, value);
    // }

    public int SectionsOuter { get; set; } = 130;
    public int SectionsWidth { get; set; } = 20;
    public NeedleVisual Needle { get; set; } = new() { Value = 0 };

    public double NeedleValue
    {
        set => Needle.Value = value;
    }

    public IEnumerable<ISeries> Series { get; set; }
    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> VisualElements { get; set; }

    public GaugeChartControl()
    {
        InitializeComponent();

        BuildSeries();
        BuildVisualElements();

        DataContext = this;
    }

    private void BuildSeries()
    {
        Series = GaugeGenerator.BuildAngularGaugeSections(
            new GaugeItem(60, s => SetStyle(SectionsOuter, SectionsWidth, s)),
            new GaugeItem(30, s => SetStyle(SectionsOuter, SectionsWidth, s)),
            new GaugeItem(10, s => SetStyle(SectionsOuter, SectionsWidth, s)));
    }

    private void BuildVisualElements()
    {
        VisualElements = new VisualElement<SkiaSharpDrawingContext>[]
        {
            new AngularTicksVisual
            {
                LabelsSize = 16,
                LabelsOuterOffset = 15,
                OuterOffset = 65,
                TicksLength = 20
            },
            Needle
        };
    }

    private static void SetStyle(
        double sectionsOuter, double sectionsWidth, PieSeries<ObservableValue> series)
    {
        series.OuterRadiusOffset = sectionsOuter;
        series.MaxRadialColumnWidth = sectionsWidth;
    }
}