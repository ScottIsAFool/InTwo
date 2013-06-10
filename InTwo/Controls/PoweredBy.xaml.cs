using System;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace InTwo.Controls
{
    public partial class PoweredBy
    {
        public PoweredBy()
        {
            InitializeComponent();
        }

        private void NokiaMusicLink_OnClick(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("nokia-music://"));
        }

        private void ScoreoidLink_OnClick(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask
            {
                Uri = new Uri("http://scoreoid.net", UriKind.Absolute)
            }.Show();
        }
    }
}
