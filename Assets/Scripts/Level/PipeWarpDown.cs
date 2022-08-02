using Core.Managers;
using Core.Player;
using UnityEngine;

namespace Level
{
	public class PipeWarpDown : MonoBehaviour {
		private LevelManager _levelManager;
		private PlayerController _mario;
		private Transform _stop;
		private bool _isMoving;

		private const float PlatformVelocityY = -0.05f;
		public string sceneName;
		public bool leadToSameLevel = true;

		// Use this for initialization
		private void Start () {
			_levelManager = FindObjectOfType<LevelManager> ();
			_mario = FindObjectOfType<PlayerController> ();
			_stop = transform.parent.transform.Find ("Platform Stop");
		}

		private void FixedUpdate()
		{
			if (!_isMoving) return;
			if (transform.position.y > _stop.position.y) {
				if (!_levelManager.GetGameStateData.TimerPaused) {
					_levelManager.GetGameStateData.TimerPaused = true;
				}

				Transform transformCached = transform;
				Vector3 position = transformCached.position;
				position = new Vector2 (position.x, position.y + PlatformVelocityY);
				transformCached.position = position;
			} else {
				if (leadToSameLevel) {
					_levelManager.GetLoadLevelSceneHandler.LoadSceneCurrentLevel (sceneName);
				} else {
					_levelManager.GetLoadLevelSceneHandler.LoadLevel(sceneName);
				}
			}
		}

		private bool _marioEntered;

		private void OnTriggerStay2D(Collider2D other) {
			if (!other.CompareTag("Player") || !_mario.IsCrouching || _marioEntered) return;
			_mario.AutomaticCrouch ();
			_isMoving = true;
			_marioEntered = true;
			_levelManager.GetSoundManager.MusicSource.Stop ();
			_levelManager.GetSoundManager.SoundSource.PlayOneShot (_levelManager.GetSoundManager.PipePowerdownSound);
		}
	}
}
