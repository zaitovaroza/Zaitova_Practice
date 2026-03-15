using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaitova.Converters
{
    public class NullableIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value as string))
                return null;

            if (int.TryParse(value as string, out int result))
                return result;

            return null;
        }
    }
}