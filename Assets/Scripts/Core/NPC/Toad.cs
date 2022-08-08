using System.Collections;
using Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPlayerController = Core.Player.PlayerController;

namespace Core.NPC
{
	public class Toad : MonoBehaviour {
		public GameObject ThankYouMario;
		public GameObject ButOurPrincess;

		private IPlayerController _playerController;
		private LevelManager _levelManager;
		
		private void Start () {
			_playerController = FindObjectOfType<IPlayerController> ();
			_levelManager = FindObjectOfType<LevelManager> ();
		}
		
		private void OnCollisionEnter2D(Collision2D other)
		{
			if (!other.gameObject.CompareTag("Player")) return;
			_playerController.GetMovementFreeze.FreezeUserInput ();
			StartCoroutine (DisplayMessageCo ());
		}

		private IEnumerator DisplayMessageCo() {
			ThankYouMario.SetActive (true);
			yield return new WaitForSecondsRealtime (.75f);
			ButOurPrincess.SetActive (true);
			yield return new WaitForSecondsRealtime (_levelManager.GetSoundManager.CastleCompleteMusic.length);
			SceneManager.LoadScene ("Main Menu");
		}
	}
}
