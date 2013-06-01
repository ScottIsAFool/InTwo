using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Scoreoid;

namespace InTwo.Model
{
    public class AppSettings : ObservableObject
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
                                        rank = "1"
                                    };
            }
            else
            {
                Messenger.Default.Register<NotificationMessage>(this, m =>
                                                                          {
                                                                              if (m.Notification.Equals(Constants.Messages.RefreshCurrentPlayerMsg))
                                                                              {
                                                                                  OnPropertyChanged("CurrentPlayer");
                                                                              }
                                                                          });
            }
        }

        public bool ShowWelcomeMessage { get; set; }
        public player CurrentPlayer { get; set; }
        public score MostRecentScore { get; set; }
    }
}
