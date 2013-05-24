using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Scoreoid;

namespace InTwo.Converters
{
    public class CurrentUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Application.Current.Resources["PhoneForegroundBrush"];

            var user = value as player;

            if (user == null) return Application.Current.Resources["PhoneForegroundBrush"];

            return (App.SettingsWrapper.AppSettings.CurrentPlayer != null &&
                    user.username == App.SettingsWrapper.AppSettings.CurrentPlayer.username)
                       ? Application.Current.Resources["PhoneAccentBrush"]
                       : Application.Current.Resources["PhoneForegroundBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
