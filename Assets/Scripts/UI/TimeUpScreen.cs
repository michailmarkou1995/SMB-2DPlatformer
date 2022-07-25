using System.Collections;
using System.Text.RegularExpressions;
using Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
	public class TimeUpScreen : MonoBehaviour {
		private GameStateManager _gameStateManager;

		public Text WorldTextHUD;
		public Text ScoreTextHUD;
		public Text CoinTextHUD;

		private float loadScreenDelay = 2;


		// Use this for initialization
		void Start () {
			Time.timeScale = 1;

			_gameStateManager = FindObjectOfType<GameStateManager> ();
			string worldName = _gameStateManager.SceneToLoad;

			WorldTextHUD.text = Regex.Split (worldName, "World ")[1];
			ScoreTextHUD.text = _gameStateManager.Scores.ToString ("D6");
			CoinTextHUD.text = "x" + _gameStateManager.Coins.ToString ("D2");

			StartCoroutine (LoadSceneDelayCo ("Level Start Screen", loadScreenDelay));
			Debug.Log (this.name + " Start: current scene is " + SceneManager.GetActiveScene ().name);
		}

		IEnumerator LoadSceneDelayCo(string sceneName, float delay = 0) {
			yield return new WaitForSecondsRealtime (delay);
			SceneManager.LoadScene (sceneName);
		}

	}
}
