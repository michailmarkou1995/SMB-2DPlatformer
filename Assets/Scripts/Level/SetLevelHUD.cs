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
        private int _timeLeftInt;


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
            get => _levelManager.GetGameStateData.Coins;
            set => _levelManager.GetGameStateData.Coins = value;
        }

        public int Scores
        {
            get => _levelManager.GetGameStateData.Scores;
            set => _levelManager.GetGameStateData.Scores = value;
        }

        public int TimeLeftIntHUD
        {
            get => _timeLeftInt;
            set => _timeLeftInt = value;
        }

        public void SetHUD()
        {
            SetHudCoin();
            SetHudScore();
            SetHudTime();
        }

        public void SetHudCoin()
        {
            coinText.text = "x" + _levelManager.GetGameStateData.Coins.ToString("D2");
        }

        public void SetHudScore()
        {
            scoreText.text = _levelManager.GetGameStateData.Scores.ToString("D6");
        }

        public void SetHudTime()
        {
            _timeLeftInt = Mathf.RoundToInt(_levelManager.GetGameStateData.TimeLeft);
            timeText.text = _timeLeftInt.ToString("D3");
        }

        public void CreateFloatingText(string text, Vector3 spawnPos)
        {
            GameObject textEffect = Instantiate(floatingTextEffect, spawnPos, Quaternion.identity);
            textEffect.GetComponentInChildren<TextMesh>().text = text.ToUpper();
        }
    }
}