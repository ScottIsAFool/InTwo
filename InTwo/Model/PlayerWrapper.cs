using Scoreoid;

namespace InTwo.Model
{
    public class PlayerWrapper : ObservableObject
    {
        public PlayerWrapper(player player)
        {
            CurrentPlayer = player;
        }

        public player CurrentPlayer { get; set; }
    }
}
