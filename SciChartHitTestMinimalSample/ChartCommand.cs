using System.Windows.Input;
using SciChart.Charting.Model.DataSeries;

namespace SciChartHitTestMinimalSample;

public class ChartCommand(XyDataSeries<double, double> dataSeries) : ICommand
{
    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        Task.Run(RenderPlot);
    }

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

        for (int i = 0; i < xValues.Length - 10; i += 10)
        {
            dataSeries.Clear();
            var (trimmedX, trimmedY) = (RemoveLeadingAndTrailingNaNs(xValues[..i]),
                RemoveLeadingAndTrailingNaNs(yValues[..i]));
            dataSeries.Append(trimmedX, trimmedY);
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

    public event EventHandler? CanExecuteChanged;
}