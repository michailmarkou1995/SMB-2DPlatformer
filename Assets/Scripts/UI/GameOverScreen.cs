using System.Collections;
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
    [RequireComponent(typeof(ILoadLevel))]
    public class GameOverScreen : MonoBehaviour, ILevelScreenSetup
    {
        [SerializeField] private Text worldTextHUD;
        [SerializeField] private Text scoreTextHUD;
        [SerializeField] private Text coinTextHUD;
        [SerializeField] private Text messageText;
        [SerializeField] private AudioSource gameOverMusicSource;
        
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

        private void Update()
        {
            if (Input.GetButton("Pause")) _levelToLoad.LoadLevel();
        }

        /// <summary>
        ///     Set the world text, score text, coin text, and game over message text.<br />
        ///     Play the game over music.
        /// </summary>
        public void SetUpLevelScreenAndWaitTillLevelLoad()
        {
            Time.timeScale = 1;

            _gameStateManager = FindObjectOfType<GameStateManager>();

            string worldName = _gameStateManager.SceneToLoad;

            // Set the world text
            worldTextHUD.text = Regex.Split(worldName, "World ")[1];
            scoreTextHUD.text = _gameStateManager.Scores.ToString("D6");
            coinTextHUD.text = "x" + _gameStateManager.Coins.ToString("D2");

            // Set the message text
            bool timeUp = _gameStateManager.TimeUp;
            if (!timeUp)
                messageText.text = "GAME OVER";
            else
                StartCoroutine(ChangeMessageCo());

            // Play the game over music
            gameOverMusicSource.volume = PlayerPrefs.GetFloat("musicVolume");
            gameOverMusicSource.Play();

            // Start the game over music wait ... then Load the Next Level e.g., main menu
            _levelToLoad.LoadLevel(loadLevelName: "Main Menu",gameOverMusicSource.clip.length);

            Debug.Log(name + " Start: current scene is " + SceneManager.GetActiveScene().name);
        }

        private IEnumerator ChangeMessageCo()
        {
            // TIME UP to GAME OVER
            messageText.text = "TIME UP";
            yield return new WaitForSecondsRealtime(1f);
            messageText.text = "GAME OVER";
        }
    }
}