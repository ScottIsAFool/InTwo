using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Nokia.Music.Types;

namespace InTwo.Controls.Settings
{
    public partial class GameDefaults
    {
        public GameDefaults()
        {
            InitializeComponent();
        }

        private void ListPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGenre = ((ListPicker)sender).SelectedItem as Genre;
            App.SettingsWrapper.AppSettings.DefaultGenre = selectedGenre;
        }
    }
}
