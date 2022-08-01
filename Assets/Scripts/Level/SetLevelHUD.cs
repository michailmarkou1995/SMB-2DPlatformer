using Interfaces.Core.Managers;
using Interfaces.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Level
{
    public class SetLevelHUD : MonoBehaviour, IHUD
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text coinText;
        [SerializeField] private Text timeText;
        [SerializeField] private GameObject floatingTextEffect;
        private const float FloatingTextOffsetY = 2f;


        public Text ScoreText => scoreText;
        public Text CoinText => coinText;
        public Text TimeText => timeText;

        private ILevelManager _levelManager;

        private void Awake()
        {
            _levelManager = GetComponent<ILevelManager>();
        }

        public int Coins
        {
            get => _levelManager.GetGameStateManager.Coins;
            set => _levelManager.GetGameStateManager.Coins = value;
        }

        public int Scores
        {
            get => _levelManager.GetGameStateManager.Scores;
            set => _levelManager.GetGameStateManager.Scores = value;
        }

        public float TimeLeft
        {
            get => _levelManager.GetGameStateManager.TimeLeft;
            set => _levelManager.GetGameStateManager.TimeLeft = value;
        }

        public int TimeLeftInt
        {
            get => _levelManager.GetGameStateManager.TimeLeftInt;
            set => _levelManager.GetGameStateManager.TimeLeftInt = value;
        }

        public void SetHUD()
        {
            SetHudCoin();
            SetHudScore();
            SetHudTime();
        }

        public void SetHudCoin()
        {
            coinText.text = "x" + _levelManager.GetGameStateManager.Coins.ToString("D2");
        }

        public void SetHudScore()
        {
            scoreText.text = _levelManager.GetGameStateManager.Scores.ToString("D6");
        }

        public void SetHudTime()
        {
            _levelManager.GetGameStateManager.TimeLeftInt =
                Mathf.RoundToInt(_levelManager.GetGameStateManager.TimeLeft);
            timeText.text = _levelManager.GetGameStateManager.TimeLeftInt.ToString("D3");
        }

        public void CreateFloatingText(string text, Vector3 spawnPos)
        {
            GameObject textEffect = Instantiate(floatingTextEffect, spawnPos, Quaternion.identity);
            textEffect.GetComponentInChildren<TextMesh>().text = text.ToUpper();
        }
    }
}