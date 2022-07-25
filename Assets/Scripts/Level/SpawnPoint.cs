using Core.Managers;
using Core.Player;
using UnityEngine;

namespace Level
{
	public class SpawnPoint : MonoBehaviour {
		private PlayerController playerController;

		// Use this for initialization
		void Start () {
			playerController = FindObjectOfType<PlayerController> ();
		}
	
		// Update is called once per frame
		void Update () {
			// update spawn pos if Player passes checkpoint
			if (playerController.gameObject.transform.position.x >= transform.position.x) {
				GameStateManager t_GameStateManager = FindObjectOfType<GameStateManager> ();
				t_GameStateManager.SpawnPointIdx = Mathf.Max (t_GameStateManager.SpawnPointIdx, gameObject.transform.GetSiblingIndex ());
				gameObject.SetActive (false);
			}

		}
	}
}
