using System;
using System.Globalization;
using System.Windows.Data;
using Scoreoid;

namespace InTwo.Converters
{
    public class ProfilePictureUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var profile = value as player;

            return profile == null ? string.Empty : string.Format(Constants.ProfilePictureUri, profile.username);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}