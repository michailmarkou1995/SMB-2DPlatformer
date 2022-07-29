using System;
using Core.Managers;
using Interfaces.Level;
using UnityEngine;

namespace Level
{
	public class Castle : MonoBehaviour {
		private LevelManager _levelManager;
		private Transform _flag;
		private Transform _flagStop;
		private bool _moveFlag;

		private const float FlagVelocityY = 0.025f;
		public string sceneName;

		private void Start () {
			_levelManager = FindObjectOfType<LevelManager> ();
			_flag = transform.Find ("Flag");
			_flagStop = transform.Find ("Flag Stop");
		}

		private void FixedUpdate()
		{
			if (!_moveFlag) return;
			if (_flag.position.y < _flagStop.position.y) {
				Vector3 position = _flag.position;
				position = new Vector2 (position.x, position.y + FlagVelocityY);
				_flag.position = position;
			} else {
				_levelManager.GetLoadLevelSceneHandler.LoadLevel(sceneName, _levelManager.GetSoundManager.LevelCompleteMusic.length);
			}
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			if (!other.gameObject.CompareTag("Player")) return;
			_moveFlag = true;
			_levelManager.MarioCompleteLevel ();
		}
	}
}
