using Core.Managers;
using UnityEngine;
using IPlayerController = Core.Player.PlayerController;

namespace Level
{
	public class PipeWarpDown : MonoBehaviour {
		private LevelManager _levelManager;
		private IPlayerController _player;
		private Transform _stop;
		private bool _isMoving;

		private const float PlatformVelocityY = -0.05f;
		public string sceneName;
		public bool leadToSameLevel = true;

		private void Start () {
			_levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
			_player = GameObject.FindGameObjectWithTag("Player").GetComponent<IPlayerController>();
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
			if (!other.CompareTag("Player") || !_player.GetCrouch.IsCrouching || _marioEntered) return;
			_player.GetCrouch.AutomaticCrouch ();
			_isMoving = true;
			_marioEntered = true;
			_levelManager.GetSoundManager.MusicSource.Stop ();
			_levelManager.GetSoundManager.SoundSource.PlayOneShot (_levelManager.GetSoundManager.PipePowerdownSound);
		}
	}
}
