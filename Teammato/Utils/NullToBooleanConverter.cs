using System.Globalization;

namespace Teammato.Utils;

public class NullToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Return true if the value is not null, otherwise false
        return value != null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Two-way binding not supported for this converter.");
    }
}