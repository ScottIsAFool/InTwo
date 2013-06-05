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
        public int NumberOfGames { get { return CurrentPlayer.scores.Length; } }
        public int TotalScore { get { return int.Parse(CurrentPlayer.bonus); } }
    }
}
