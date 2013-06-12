using System;
using System.Windows;
using Anotar.MetroLog;
using Microsoft.Phone.Controls;

namespace InTwo.Views.Welcome
{
    public partial class WelcomeView : PhoneApplicationPage
    {
        // Constructor
        public WelcomeView()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            Navigate(new Uri(Constants.Pages.Welcome.ScoreoidWelcome, UriKind.Relative));
        }

        private void NoThanksButton_OnClick(object sender, RoutedEventArgs e)
        {
            FlurryWP8SDK.Api.LogEvent("TermsDeclined");
            Application.Current.Terminate();
        }

        private void Navigate(Uri link)
        {
            Log.Info("Navigating to " + link);
            NavigationService.Navigate(link);
        }
    }
}