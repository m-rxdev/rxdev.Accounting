using System;
using System.Windows.Data;
using System.Windows.Media;

namespace rxdev.Accounting.App.Resources.Converters;

[ValueConversion(typeof(decimal), typeof(Brush))]
public class AmountToBrushConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is not decimal val)
            return null;

        if (val < 0)
            return Brushes.Red;
        else if (val == 0)
            return Brushes.DarkGray;

        return Brushes.Green;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}