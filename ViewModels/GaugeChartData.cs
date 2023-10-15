using System.Collections.Generic;
using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using BlackWater.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;

namespace BlackWater.ViewModels;

public class GaugeChartData : ObservableObject
{
    public string? ChartTitle { get; set; } = "SET TITLE";
    public int SectionsOuter { get; set; } = 130;
    public int SectionsWidth { get; set; } = 20;

    public NeedleVisual Needle { get; set; } = new() { Value = 0 };
    public IEnumerable<ISeries> Series { get; set; }
    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> VisualElements { get; set; }

    public GaugeChartData()
    {
        InitChartEngine();
    }

    private void InitChartEngine()
    {
        BuildSeries();
        BuildVisualElements();
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