using System;
using System.Globalization;
using System.Windows.Data;

namespace rxdev.Accounting.App.Resources.Converters;

[ValueConversion(typeof(double), typeof(double))]
public class DoubleMultiplicationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => GetDouble(value) * GetDouble(parameter);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => GetDouble(value) / GetDouble(parameter);

    private static double GetDouble(object value)
        => value is double dValue
        ? dValue
        : double.Parse(value?.ToString() ?? "0");
}