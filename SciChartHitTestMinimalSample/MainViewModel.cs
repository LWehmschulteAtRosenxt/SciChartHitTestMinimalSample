using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Rosen.Tech.Wpf.Commands;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.PointMarkers;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Core.Extensions;

namespace SciChartHitTestMinimalSample;

public class MainViewModel : INotifyPropertyChanged
{
    private string _chartTitle = "Hello SciChart World!";
    private string _xAxisTitle = "XAxis";
    private string _yAxisTitle = "YAxis";
    private ObservableCollection<IRenderableSeriesViewModel> _renderableSeries = [];
    private readonly XyDataSeries<double, double> lineData;

    public string ChartTitle
    {
        get => _chartTitle;
        set
        {
            _chartTitle = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string XAxisTitle
    {
        get => _xAxisTitle;
        set
        {
            _xAxisTitle = value;
            OnPropertyChanged();
        }
    }

    public string YAxisTitle
    {
        get => _yAxisTitle;
        set
        {
            _yAxisTitle = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<IRenderableSeriesViewModel> RenderableSeries
    {
        get => _renderableSeries;
        set
        {
            _renderableSeries = value;
            OnPropertyChanged();
        }
    }

    public ICommand RenderCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private (double[] XValues, double[] YValues) GenerateData()
    {
        var length = 100;
        var amp = 5;
        var freq = 0.5;
        var interval = 0.01;
        var numberOfPoints = (int)(length / interval);

        var xValues = new double[numberOfPoints];
        var yValues = new double[numberOfPoints];

        for (var i = 0d; i < length; i += interval)
        {
            var index = (int)(Math.Round(i / interval));

            if (i % 10 is > 0 and < 2)
            {
                xValues[index] = double.NaN;
                yValues[index] = double.NaN;
                continue;
            }

            var xVal = i;
            var yVal = amp * (Math.Sin(i * freq) / (i + 0.01));

            xValues[index] = xVal;
            yValues[index] = yVal;
        }

        return (xValues, yValues);
    }

    public async Task RenderPlot()
    {
        var (xValues, yValues) = GenerateData();
        lineData.Capacity = xValues.Length;

        for (int i = 0; i < xValues.Length - 10; i += 10)
        {
            lineData.Clear();
            var (trimmedX, trimmedY) = (RemoveLeadingAndTrailingNaNs(xValues[..i]),
                RemoveLeadingAndTrailingNaNs(yValues[..i]));
            lineData.Append(trimmedX, trimmedY);
            await Task.Delay(10);
        }
    }

    private double[] RemoveLeadingAndTrailingNaNs(double[] values)
    {
        var start = 0;
        var end = values.Length - 1;

        while (start <= end && double.IsNaN(values[start]))
        {
            start++;
        }

        while (end >= start && double.IsNaN(values[end]))
        {
            end--;
        }

        if (start > end)
        {
            return [];
        }

        var length = end - start + 1;
        var trimmedArray = new double[length];
        Array.Copy(values, start, trimmedArray, 0, length);

        return trimmedArray;
    }

    public MainViewModel()
    {
        this.RenderCommand = new AsyncCommand(this.RenderPlot);

        this.lineData = new XyDataSeries<double, double>()
        {
            SeriesName = "TestingSeries",
            AcceptsUnsortedData = true,
        };

        this.RenderableSeries.Add(new LineRenderableSeriesViewModel()
        {
            StrokeThickness = 2,
            Stroke = Colors.SteelBlue,
            DataSeries = lineData,
            DrawNaNAs = LineDrawMode.Gaps,
            IncludeTooltipModifier = true
        });
    }
}