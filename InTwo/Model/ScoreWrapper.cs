using PropertyChanged;

using ScoreoidPortable.Entities;

namespace InTwo.Model
{
    [ImplementPropertyChanged]
    public class ScoreWrapper
    {
        public ScoreItem Score { get; set; }
        public int Rank { get; set; }
        public ScoreInfo ScoreInfo { get; set; }
    }
}
