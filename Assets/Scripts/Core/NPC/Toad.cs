using System.Collections;
using Core.Managers;
using Core.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.NPC
{
	public class Toad : MonoBehaviour {
		public GameObject ThankYouMario;
		public GameObject ButOurPrincess;

		private PlayerController playerController;
		private LevelManager t_LevelManager;


		// Use this for initialization
		void Start () {
			playerController = FindObjectOfType<PlayerController> ();
			t_LevelManager = FindObjectOfType<LevelManager> ();
		}


		void OnCollisionEnter2D(Collision2D other) {
			if (other.gameObject.tag == "Player") {
				playerController.FreezeUserInput ();
				StartCoroutine (DisplayMessageCo ());
			}
		}

		IEnumerator DisplayMessageCo() {
			ThankYouMario.SetActive (true);
			yield return new WaitForSecondsRealtime (.75f);
			ButOurPrincess.SetActive (true);
			yield return new WaitForSecondsRealtime (t_LevelManager.GetSoundManager.CastleCompleteMusic.length);
			SceneManager.LoadScene ("Main Menu");
		}
	}
}
