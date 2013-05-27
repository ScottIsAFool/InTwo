using System.Collections.Generic;
using Nokia.Music.Types;

namespace InTwo.Model
{
    public class GameData : ObservableObject
    {
        public GameData()
        {
            Genres = new List<Genre>();
        }

        public List<Genre> Genres { get; set; }
    }
}
