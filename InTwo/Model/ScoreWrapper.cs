using PropertyChanged;

using ScoreoidPortable.Entities;

namespace InTwo.Model
{
    [ImplementPropertyChanged]
    public class ScoreWrapper
    {
        public Score Score { get; set; }
        public ScoreInfo ScoreInfo { get; set; }
    }
}
