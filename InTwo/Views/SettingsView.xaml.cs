using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Nokia.Music.Types;

namespace InTwo.Views
{
    public partial class SettingsView : PhoneApplicationPage
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
    }
}