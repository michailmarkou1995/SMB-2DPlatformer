using UnityEngine;

namespace Core
{
	public class SpawnPoint : MonoBehaviour {
		private PlayerController mario;

		// Use this for initialization
		void Start () {
			mario = FindObjectOfType<PlayerController> ();
		}
	
		// Update is called once per frame
		void Update () {
			// update spawn pos if Player passes checkpoint
			if (mario.gameObject.transform.position.x >= transform.position.x) {
				GameStateManager t_GameStateManager = FindObjectOfType<GameStateManager> ();
				t_GameStateManager.spawnPointIdx = Mathf.Max (t_GameStateManager.spawnPointIdx, gameObject.transform.GetSiblingIndex ());
				gameObject.SetActive (false);
			}

		}
	}
}
