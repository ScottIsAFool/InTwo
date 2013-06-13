using System;
using Nokia.Music.Types;

namespace InTwo.Model
{
    public class GameState
    {
        public GameState(){}

        public GameState(TimeSpan gameLength, Genre gameGenre, int roundNumber, int currentScore)
        {
            GameGenre = gameGenre;
            GameLength = gameLength;
            CurrentScore = currentScore;
            RoundNumber = roundNumber;
        }

        public TimeSpan GameLength { get; set; }
        public Genre GameGenre { get; set; }
        public int RoundNumber { get; set; }
        public int CurrentScore { get; set; }
    }
}
