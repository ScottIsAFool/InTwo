using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace InTwo.Views.Welcome
{
    public partial class ScoreoidWelcomeView : PhoneApplicationPage
    {
        // Constructor
        public ScoreoidWelcomeView()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Constants.Pages.Welcome.DownloadSongsNow, UriKind.Relative));
        }
    }
}