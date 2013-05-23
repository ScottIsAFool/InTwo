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