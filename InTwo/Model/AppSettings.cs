using System.ComponentModel;
using GalaSoft.MvvmLight;
using Scoreoid;

namespace InTwo.Model
{
    public class AppSettings : INotifyPropertyChanged
    {
        public AppSettings()
        {
            ShowWelcomeMessage = true;
            if (ViewModelBase.IsInDesignModeStatic)
            {
                CurrentPlayer = new player
                    {
                        username = "scottisafool",
                        best_score = "336",
                        rank= "1"
                    };
            }
        }

        public bool ShowWelcomeMessage { get; set; }
        public player CurrentPlayer { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
