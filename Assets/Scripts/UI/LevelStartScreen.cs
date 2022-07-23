using System.Collections;
using System.Text.RegularExpressions;
using Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Text))]
    public class LevelStartScreen : MonoBehaviour
    {
        private GameStateManager _gameStateManager;
        private const float LoadScreenDelay = 2;

        public Text worldTextHUD;
        public Text scoreTextHUD;
        public Text coinTextHUD;
        public Text worldTextMain;
        public Text livesText;

        // Use this for initialization
        private void Start()
        {
            Time.timeScale = 1;

            _gameStateManager = FindObjectOfType<GameStateManager>();
            string worldName = _gameStateManager.sceneToLoad;

            worldTextHUD.text = Regex.Split(worldName, "World ")[1];
            scoreTextHUD.text = _gameStateManager.scores.ToString("D6");
            coinTextHUD.text = "x" + _gameStateManager.coins.ToString("D2");
            worldTextMain.text = worldName.ToUpper();
            livesText.text = _gameStateManager.lives.ToString();

            StartCoroutine(LoadSceneDelayCo(_gameStateManager.sceneToLoad, LoadScreenDelay));

            Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
        }

        private static IEnumerator LoadSceneDelayCo(string sceneName, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            SceneManager.LoadScene(sceneName);
        }
    }
}