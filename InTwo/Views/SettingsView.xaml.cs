using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Nokia.Music.Types;

namespace InTwo.Views
{
    public partial class SettingsView 
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ListPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGenre = ((ListPicker) sender).SelectedItem as Genre;
            App.SettingsWrapper.AppSettings.DefaultGenre = selectedGenre;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ForceSettingsSaveMsg));
        }
    }
}