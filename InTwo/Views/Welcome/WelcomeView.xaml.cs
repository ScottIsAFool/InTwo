using System;
using System.Windows;
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
            NavigationService.Navigate(new Uri(Constants.Pages.Welcome.ScoreoidWelcome, UriKind.Relative));
        }
    }
}