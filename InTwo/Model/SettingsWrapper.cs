using System.ComponentModel;

namespace InTwo.Model
{
    public class SettingsWrapper : INotifyPropertyChanged
    {
        public SettingsWrapper()
        {
            AppSettings = new AppSettings();
        }

        public AppSettings AppSettings { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}