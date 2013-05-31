using System;
using System.Globalization;
using System.Windows.Data;

namespace InTwo.Converters
{
    public class UserLoggedInConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return App.CurrentPlayer != null ? "log out" : "log in";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
