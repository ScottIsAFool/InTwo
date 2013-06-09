using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Scoreoid;

namespace InTwo.Model
{
    public class PlayerWrapper : ObservableObject
    {
        public PlayerWrapper(player player)
        {
            CurrentPlayer = player;
        }

        public PlayerWrapper()
        {
            if (!ViewModelBase.IsInDesignModeStatic)
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

        public player CurrentPlayer { get; set; }
        public int NumberOfGames { get { return !string.IsNullOrEmpty(CurrentPlayer.boost) ? int.Parse(CurrentPlayer.boost) : 0; } }
        public int TotalScore { get { return !string.IsNullOrEmpty(CurrentPlayer.bonus) ? int.Parse(CurrentPlayer.bonus) : 0; } }
        public string BestScoreGenre { get { return CurrentPlayer.last_level; } }
        public string BestScore { get { return CurrentPlayer.best_score; } }
    }
}
