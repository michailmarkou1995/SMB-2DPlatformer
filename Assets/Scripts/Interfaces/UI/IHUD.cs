using UnityEngine;
using UnityEngine.UI;

namespace Interfaces.UI
{
    public interface IHUD
    {
        public Text ScoreText { get; }
        public Text CoinText { get; }
        public Text TimeText { get; }
        public int Coins { get; set; }
        public int Scores { get; set; }
        public float TimeLeft { get; set; }
        public int TimeLeftInt { get; set; }
        public void SetHUD();
        public void SetHudCoin();
        public void SetHudScore();
        public void SetHudTime();
        public void CreateFloatingText(string text, Vector3 spawnPosition);
    }
}