using System.Collections;
using System.Text.RegularExpressions;
using Core.Managers;
using UI.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameOverScreen : MonoBehaviour//, IGameOverScreen
    {
        private GameStateManager _gameStateManager;

        [SerializeField] private Text worldTextHUD;
        [SerializeField] private Text scoreTextHUD;
        [SerializeField] private Text coinTextHUD;
        [SerializeField] private Text messageText;

        [SerializeField] private AudioSource gameOverMusicSource;

        private void Start()
        {
            //_gameStateManager = GetComponent<IGameStateManager>();
            SetUpGameOverScreen();
        }

        private void SetUpGameOverScreen()
        {
            Time.timeScale = 1;

            _gameStateManager = FindObjectOfType<GameStateManager>();
            string worldName = _gameStateManager.sceneToLoad;

            worldTextHUD.text = Regex.Split(worldName, "World ")[1];
            scoreTextHUD.text = _gameStateManager.scores.ToString("D6");
            coinTextHUD.text = "x" + _gameStateManager.coins.ToString("D2");

            bool timeUp = _gameStateManager.timeUp;
            if (!timeUp) {
                messageText.text = "GAME OVER";
            } else {
                StartCoroutine(ChangeMessageCo());
            }

            gameOverMusicSource.volume = PlayerPrefs.GetFloat("musicVolume");
            gameOverMusicSource.Play();
            LoadMainMenu(gameOverMusicSource.clip.length);

            Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
        }

        private static IEnumerator LoadSceneDelayCo(string sceneName, float delay = 0)
        {
            yield return new WaitForSecondsRealtime(delay);
            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator ChangeMessageCo()
        {
            // TIME UP to GAME OVER
            messageText.text = "TIME UP";
            yield return new WaitForSecondsRealtime(1f);
            messageText.text = "GAME OVER";
        }

        private void Update()
        {
            if (Input.GetButton("Pause")) {
                LoadMainMenu();
            }
        }

        private void LoadMainMenu(float delay = 0)
        {
            StartCoroutine(LoadSceneDelayCo("Main Menu", delay));
        }
    }
}