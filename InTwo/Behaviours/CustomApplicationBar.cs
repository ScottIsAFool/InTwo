using Cimbalino.Phone.Toolkit.Behaviors;

namespace InTwo.Behaviours
{
    public class CustomApplicationBar : ApplicationBar
    {
        public CustomApplicationBar()
        {
            Opacity = 0.4;
        }
    }

    public class CustomApplicationBarBehaviour : ApplicationBarBehavior
    {
        public CustomApplicationBarBehaviour()
        {
            Opacity = 0.4;
        }
    }
}
