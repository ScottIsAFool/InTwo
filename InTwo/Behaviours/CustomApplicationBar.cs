using System.Windows.Media;
using Cimbalino.Phone.Toolkit.Behaviors;

namespace InTwo.Behaviours
{
    public class CustomApplicationBar : ApplicationBar
    {
        public CustomApplicationBar()
        {
            Opacity = 0.4;
            BackgroundColor = (Color) App.Current.Resources["AppBarColour"];
        }
    }

    public class CustomApplicationBarBehaviour : ApplicationBarBehavior
    {
        public CustomApplicationBarBehaviour()
        {
            Opacity = 0.4;
            BackgroundColor = (Color)App.Current.Resources["AppBarColour"];
        }
    }
}
