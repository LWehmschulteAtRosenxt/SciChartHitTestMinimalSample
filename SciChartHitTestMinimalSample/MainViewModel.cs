using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;

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

    public MainViewModel()
    {

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

        this.RenderCommand = new ChartCommand(lineData);
    }
}