using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace InTwo.Views
{
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ForceSettingsSaveMsg));
        }
    }
}