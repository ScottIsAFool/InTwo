using System.Windows;
using System.Windows.Media;
using Cimbalino.Phone.Toolkit.Behaviors;
using Microsoft.Phone.Shell;
using ApplicationBar = Cimbalino.Phone.Toolkit.Behaviors.ApplicationBar;

namespace InTwo.Behaviours
{
    public class CustomApplicationBar : ApplicationBar
    {
        public CustomApplicationBar()
        {
            Opacity = 0.4;
            BackgroundColor = (Color) Application.Current.Resources["AppBarColour"];
            ForegroundColor = Colors.White;
            StateChanged += OnStateChanged;
        }

        private void OnStateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            Opacity = e.IsMenuVisible ? 1 : 0.4;
        }
    }

    public class CustomApplicationBarBehaviour : ApplicationBarBehavior
    {
        public CustomApplicationBarBehaviour()
        {
            Opacity = 0.4;
            BackgroundColor = (Color)Application.Current.Resources["AppBarColour"];
            ForegroundColor = Colors.White;
            StateChanged += OnStateChanged;
        }

        private void OnStateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            Opacity = e.IsMenuVisible ? 1 : 0.4;
        }
    }
}
