using UnityEngine.UI;

namespace Interfaces.UI
{
    public interface ILevelStartScreenEssentials
    {
        public Text WorldTextHUD { get; set; }
        public Text ScoreTextHUD { get; set; }
        public Text CoinTextHUD { get; set; }
        public Text WorldTextMain { get; set; }
        public Text LivesText { get; set; }
    }
}