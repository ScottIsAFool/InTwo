using System.ComponentModel;

namespace InTwo.Model
{
    public class AppSettings : INotifyPropertyChanged
    {
        public AppSettings()
        {
            ShowWelcomeMessage = true;
        }

        public bool ShowWelcomeMessage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
