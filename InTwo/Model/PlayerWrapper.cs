using Scoreoid;

namespace InTwo.Model
{
    public class PlayerWrapper : ObservableObject
    {
        public PlayerWrapper(player player)
        {
            CurrentPlayer = player;
        }

        public PlayerWrapper(){}

        public player CurrentPlayer { get; set; }
        public int NumberOfGames { get { return !string.IsNullOrEmpty(CurrentPlayer.boost) ? int.Parse(CurrentPlayer.boost) : 0; } }
        public int TotalScore { get { return !string.IsNullOrEmpty(CurrentPlayer.bonus) ? int.Parse(CurrentPlayer.bonus) : 0; } }
        public string BestScoreGenre { get { return CurrentPlayer.last_level; } }
        public string BestScore { get { return CurrentPlayer.best_score; } }
    }
}
