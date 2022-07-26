using System;
using System.Text.RegularExpressions;
using Core.Managers;
using Interfaces.Core.Managers;
using Interfaces.Level;
using Interfaces.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(ILevelScreenSetup))]
    public class TimeUpScreen : MonoBehaviour, ILevelScreenSetup
    {
        [SerializeField] private Text worldTextHUD;
        [SerializeField] private Text scoreTextHUD;
        [SerializeField] private Text coinTextHUD;

        private const float LoadScreenDelay = 2;

        private IGameStateManagerEssentials _gameStateManager;
        private ILoadLevel _levelToLoad;

        private void Awake()
        {
            _levelToLoad = GetComponent<ILoadLevel>();
        }

        private void Start()
        {
            SetUpLevelScreenAndWaitTillLevelLoad();
        }

        public void SetUpLevelScreenAndWaitTillLevelLoad()
        {
            Time.timeScale = 1;

            _gameStateManager = FindObjectOfType<GameStateManager>();
            string worldName = _gameStateManager.SceneToLoad;

            worldTextHUD.text = Regex.Split(worldName, "World ")[1];
            scoreTextHUD.text = _gameStateManager.Scores.ToString("D6");
            coinTextHUD.text = "x" + _gameStateManager.Coins.ToString("D2");

            _levelToLoad.LoadLevel("Level Start Screen", LoadScreenDelay);

            Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
        }
    }
}