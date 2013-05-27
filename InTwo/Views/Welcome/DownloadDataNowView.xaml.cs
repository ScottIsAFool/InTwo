using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace InTwo.Views.Welcome
{
    public partial class DownloadDataNowView : PhoneApplicationPage
    {
        // Constructor
        public DownloadDataNowView()
        {
            InitializeComponent();
        }

        private void NotNowButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Constants.Pages.MainPage + Constants.ClearBackStack, UriKind.Relative));
        }

        private void DownloadDataButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Constants.Pages.DownloadingSongs, UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                App.SettingsWrapper.AppSettings.ShowWelcomeMessage = false;
            }
        }
    }
}