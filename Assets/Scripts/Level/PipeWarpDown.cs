using Core.Managers;
using Core.Player;
using UnityEngine;

namespace Level
{
	public class PipeWarpDown : MonoBehaviour {
		private LevelManager tLevelManager;
		private PlayerController mario;
		private Transform stop;
		private bool isMoving;

		private const float PlatformVelocityY = -0.05f;
		public string sceneName;
		public bool leadToSameLevel = true;

		// Use this for initialization
		private void Start () {
			tLevelManager = FindObjectOfType<LevelManager> ();
			mario = FindObjectOfType<PlayerController> ();
			stop = transform.parent.transform.Find ("Platform Stop");
		}

		private void FixedUpdate() {
			if (isMoving) {
				if (transform.position.y > stop.position.y) {
					if (!tLevelManager.timerPaused) {
						tLevelManager.timerPaused = true;
					}

					Transform transformCached = transform;
					Vector3 position = transformCached.position;
					position = new Vector2 (position.x, position.y + PlatformVelocityY);
					transformCached.position = position;
				} else {
					if (leadToSameLevel) {
						tLevelManager.LoadSceneCurrentLevel (sceneName);
					} else {
						tLevelManager.LoadNewLevel (sceneName);
					}
				}
			}
		}

		private bool marioEntered;

		private void OnTriggerStay2D(Collider2D other) {
			if (!other.CompareTag("Player") || !mario.IsCrouching || marioEntered) return;
			mario.AutomaticCrouch ();
			isMoving = true;
			marioEntered = true;
			tLevelManager.GetSoundManager.MusicSource.Stop ();
			tLevelManager.GetSoundManager.SoundSource.PlayOneShot (tLevelManager.GetSoundManager.PipePowerdownSound);
		}
	}
}
