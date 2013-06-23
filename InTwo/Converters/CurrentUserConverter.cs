using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ScoreoidPortable.Entities;

namespace InTwo.Converters
{
    public class CurrentUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Application.Current.Resources["PhoneForegroundBrush"];

            var user = value as Player;

            if (user == null) return Application.Current.Resources["PhoneForegroundBrush"];

            return (App.SettingsWrapper.AppSettings.PlayerWrapper.CurrentPlayer != null &&
                    user.Username == App.SettingsWrapper.AppSettings.PlayerWrapper.CurrentPlayer.Username)
                       ? Application.Current.Resources["PhoneAccentBrush"]
                       : Application.Current.Resources["PhoneForegroundBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
