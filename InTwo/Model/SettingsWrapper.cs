using System.ComponentModel;

namespace InTwo.Model
{
    public class SettingsWrapper : ObservableObject
    {
        public SettingsWrapper()
        {
            AppSettings = new AppSettings();
        }

        public AppSettings AppSettings { get; set; }
    }
}