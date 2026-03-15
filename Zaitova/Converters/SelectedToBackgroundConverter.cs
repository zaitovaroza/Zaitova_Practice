using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Zaitova.Converters
{
    public class SelectedToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")); // Серый

            return new SolidColorBrush(Colors.White); // Белый
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}