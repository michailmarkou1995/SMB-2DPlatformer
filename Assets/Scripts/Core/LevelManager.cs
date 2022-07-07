using UnityEngine;
using UnityEngine.UI;

namespace Core
{
	public class LevelManager : MonoBehaviour {
		private PlayerController mario;
		private GameStateManager t_GameStateManager;

		void Awake() {
			Time.timeScale = 1;
		}

		// Use this for initialization
		void Start () {
			t_GameStateManager = FindObjectOfType<GameStateManager>();
			mario = FindObjectOfType<PlayerController> ();
		}
		
		/****************** Misc */
		public Vector3 FindSpawnPosition() {
			Vector3 spawnPosition;
			GameStateManager tGameStateManager = FindObjectOfType<GameStateManager>();
			Debug.Log (this.name + " FindSpawnPosition: GSM spawnFromPoint=" + tGameStateManager.spawnFromPoint.ToString()
			           + " spawnPipeIdx= " + tGameStateManager.spawnPipeIdx.ToString() 
			           + " spawnPointIdx=" + tGameStateManager.spawnPointIdx.ToString());
			if (tGameStateManager.spawnFromPoint) {
				spawnPosition = GameObject.Find ("Spawn Points").transform.GetChild (tGameStateManager.spawnPointIdx).transform.position;
			} else {
				spawnPosition = GameObject.Find ("Spawn Pipes").transform.GetChild (tGameStateManager.spawnPipeIdx).transform.Find("Spawn Pos").transform.position;
			}
			return spawnPosition;
		}
	}
}
