using System;
using System.Globalization;
using System.Windows.Data;

namespace InTwo.Converters
{
    public class SecondsTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "2 seconds";

            var seconds = int.Parse(value.ToString());

            return seconds == 1 ? "1 second" : seconds + " seconds";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
