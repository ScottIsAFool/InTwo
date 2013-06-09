using System.ComponentModel;
using System.Windows.Media;

namespace InTwo.Model
{
    public class SettingsWrapper : ObservableObject
    {
        public SettingsWrapper()
        {
            AppSettings = new AppSettings();
        }

        public AppSettings AppSettings { get; set; }
        public bool HasRemovedAds { get; set; }
        public SolidColorBrush UsersAccentBrush { get; set; }

        public void OnUsersAccentBrushChanged()
        {
            var s = "";
        }
    }
}