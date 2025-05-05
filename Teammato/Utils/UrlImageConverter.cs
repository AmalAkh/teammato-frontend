using System.Globalization;

namespace Teammato.Utils;
using Teammato.Services;


public class UrlImageConverter:IValueConverter
{
    
  

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string filename && !string.IsNullOrWhiteSpace(filename))
        {
            return new Uri(RestAPIService.BaseAddress+"static/" + filename);
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // Typically not needed for one-way binding
    }
    
}