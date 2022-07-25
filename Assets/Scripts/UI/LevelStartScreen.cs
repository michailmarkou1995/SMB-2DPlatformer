using System;
using System.Text.RegularExpressions;
using Core.Managers;
using Interfaces.Level;
using Interfaces.UI;
using Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    #region TestCode

    // public interface ILevelStartScreenEssentials
    // {
    //     public GameStateManager StateManager { get; }
    //
    //     private const float LoadScreenDelay = 2f;
    //
    //     public Text WorldTextHUD { get; set; }
    //     public Text ScoreTextHUD { get; set; }
    //     public Text CoinTextHUD { get; set; }
    //     public Text WorldTextMain { get; set; }
    //     public Text LivesText { get; set; }
    //     public int Test { get; set; }
    // }
    //
    // public interface ILevelStartScreenProvider
    // {
    //     ILevelStartScreenEssentials GetEssentials { get;}
    // }

    #endregion

    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(ILoadLevel))]
    public class LevelStartScreen : LevelStartScreenBase, ILevelStartScreenEssentials
    {
        #region GettersAndSetters

        public Text WorldTextHUD
        {
            get => worldTextHUD;
            set => worldTextHUD = value;
        }

        public Text ScoreTextHUD
        {
            get => scoreTextHUD;
            set => scoreTextHUD = value;
        }

        public Text CoinTextHUD
        {
            get => coinTextHUD;
            set => coinTextHUD = value;
        }

        public Text WorldTextMain
        {
            get => worldTextMain;
            set => worldTextMain = value;
        }

        public Text LivesText
        {
            get => livesText;
            set => livesText = value;
        }

        #endregion

        // Object Compositions - Dependencies
        private GameStateManager _gameStateManager;
        private ILoadLevel _loadNextLevel;

        private void Awake()
        {
            _loadNextLevel = GetComponent<ILoadLevel>();
        }

        private void Start()
        {
            SetUpLevelStartScreen();
        }

        private void SetUpLevelStartScreen()
        {
            // Time mode slow down
            Time.timeScale = 1;

            // Get the game state manager
            _gameStateManager = FindObjectOfType<GameStateManager>();
            string worldName = _gameStateManager.SceneToLoad;

            // Set text
            WorldTextHUD.text = Regex.Split(worldName, "World ")[1];
            ScoreTextHUD.text = _gameStateManager.Scores.ToString("D6");
            CoinTextHUD.text = "x" + _gameStateManager.Coins.ToString("D2");
            WorldTextMain.text = worldName.ToUpper();
            LivesText.text = _gameStateManager.Lives.ToString();

            // Get the load next level script
            _loadNextLevel.LoadLevel(_gameStateManager.SceneToLoad, LoadScreenDelay);

            Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
        }
    }
}