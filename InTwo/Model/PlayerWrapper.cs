using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ScoreoidPortable.Entities;


namespace InTwo.Model
{
    public class PlayerWrapper : ObservableObject
    {
        public PlayerWrapper(Player player)
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

        public Player CurrentPlayer { get; set; }
        public int NumberOfGames { get { return !string.IsNullOrEmpty(CurrentPlayer.Boost) ? int.Parse(CurrentPlayer.Boost) : 0; } }
        public int TotalScore { get { return CurrentPlayer.Bonus; } }
        public string BestScoreGenre { get { return CurrentPlayer.LastLevel; } }
        public int BestScore { get { return CurrentPlayer.BestScore; } }
    }
}
