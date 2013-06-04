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
        public int NumberOfGames { get; set; }
        public int TotalScore { get; set; }
    }
}
