using System.Globalization;
using Teammato.Services;

namespace Teammato.Utils;

public class IGDBUrlImageConverter:IValueConverter
{






    
  

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string filename && !string.IsNullOrWhiteSpace(filename))
        {
            return new Uri("https://images.igdb.com/igdb/image/upload/t_cover_big/" + filename);
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // Typically not needed for one-way binding
    }
    

}