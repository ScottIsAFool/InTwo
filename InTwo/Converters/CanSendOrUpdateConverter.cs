using System;
using System.Globalization;
using System.Windows.Data;

namespace InTwo.Converters
{
    public class CanSendOrUpdateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;

            var player = (string) value;

            return !string.IsNullOrEmpty(player);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}