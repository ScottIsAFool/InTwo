using System;
using System.Windows;

namespace InTwo.Views.Welcome
{
    public partial class ScoreoidWelcomeView 
    {
        // Constructor
        public ScoreoidWelcomeView()
        {
            InitializeComponent();
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Constants.Pages.Welcome.DownloadSongsNow, UriKind.Relative));
        }
    }
}