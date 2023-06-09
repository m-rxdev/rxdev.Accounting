using System;
using System.Globalization;
using System.Windows.Data;

namespace rxdev.Accounting.App.Resources.Converters;

public class BindingValidatorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => parameter;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}