using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace InTwo.Views.Welcome
{
    public partial class StopMusicView : PhoneApplicationPage
    {
        public StopMusicView()
        {
            InitializeComponent();
        }

        private void NoThanksButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Constants.Pages.Welcome.DownloadSongsNow, UriKind.Relative));
        }

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            App.SettingsWrapper.AppSettings.DontShowAllowStopMusicMessage = true;
            App.SettingsWrapper.AppSettings.AllowStopMusic = true;

            NavigationService.Navigate(new Uri(Constants.Pages.Welcome.DownloadSongsNow, UriKind.Relative));
        }
    }
}