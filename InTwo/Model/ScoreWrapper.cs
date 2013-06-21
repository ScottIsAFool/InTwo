using PropertyChanged;
using Scoreoid;

namespace InTwo.Model
{
    [ImplementPropertyChanged]
    public class ScoreWrapper
    {
        public score Score { get; set; }
        public ScoreInfo ScoreInfo { get; set; }
    }
}
