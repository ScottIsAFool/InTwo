using System;
using System.Windows;
using Anotar.MetroLog;
using Microsoft.Phone.Controls;

namespace InTwo.Views.Welcome
{
    public partial class StopMusicView 
    {
        public StopMusicView()
        {
            InitializeComponent();
        }

        private void NoThanksButton_OnClick(object sender, RoutedEventArgs e)
        {
            Navigate(new Uri(Constants.Pages.Welcome.DownloadSongsNow, UriKind.Relative));
        }

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            App.SettingsWrapper.AppSettings.DontShowAllowStopMusicMessage = true;
            App.SettingsWrapper.AppSettings.AllowStopMusic = true;

            Navigate(new Uri(Constants.Pages.Welcome.DownloadSongsNow, UriKind.Relative));
        }

        private void Navigate(Uri link)
        {
            Log.Info("Navigating to " + link);
            NavigationService.Navigate(link);
        }
    }
}