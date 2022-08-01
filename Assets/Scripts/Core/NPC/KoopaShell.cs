using Core.Managers;
using Core.Player;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Core.NPC
{
	public class KoopaShell : Enemy {
		private Animator _animator;
		private Rigidbody2D _rigidbody2D;
		private ILevelManager _levelManager;
		private PlayerController _playerController;

		public GameObject Koopa;
		public float rollSpeedX = 7;
		private float _waitTillRevive = 5;
		private float _waitTillRespawn = 1.5f;

		private float _currentRollVelocityX;
		private bool _isReviving;
		public bool isRolling;

		private void Start () {
			_levelManager = FindObjectOfType<LevelManager> ();
			_playerController = FindObjectOfType<PlayerController> ();
			_animator = GetComponent<Animator> ();
			_rigidbody2D = GetComponent<Rigidbody2D> ();
			_isReviving = false;
			isRolling = false;

			starmanBonus = 200; // ???
			rollingShellBonus = 500; // ???
			hitByBlockBonus = 100; // ???
			fireballBonus = 100; // ???
			stompBonus = 500;
		}

		private void Update() {
			if (!_isReviving && !isRolling) {
				_waitTillRevive -= Time.deltaTime;
				if (_waitTillRevive <= 0) {
					_animator.SetTrigger ("revived");
					_isReviving = true;
				}
			} else if (_isReviving && !isRolling) {
				_waitTillRespawn -= Time.deltaTime;
				if (_waitTillRespawn <= 0) {
					Instantiate (Koopa, transform.position, Quaternion.identity);
					Destroy (gameObject);
				}
			} else if (isRolling) {
				_rigidbody2D.velocity = new Vector2 (_currentRollVelocityX, _rigidbody2D.velocity.y);
			}

			if (_hasBeenStomped) {
				stompBonus = 0;
			}
		}

		public override void TouchedByRollingShell() {
			if (!isRolling) {
				FlipAndDie ();
			} else { // change direction if touched by another rolling shell
				_currentRollVelocityX = -_currentRollVelocityX;
				rollingShellBonus = 0; // ???
			}
		}

		private bool _hasBeenStomped;
		private static readonly int Rolled = Animator.StringToHash("rolled");

		public override void StompedByMario() {
			isBeingStomped = true;
			if (!isRolling) {
				// start rolling left/right depending on Mario's direction
				if (_playerController.transform.localScale.x == 1) {
					_currentRollVelocityX = rollSpeedX;
				} else if (_playerController.transform.localScale.x == -1) {
					_currentRollVelocityX = -rollSpeedX;
				}
				isRolling = true;
				_animator.SetTrigger (Rolled);
			} else {
				isRolling = false;
			}
			_hasBeenStomped = true;
			isBeingStomped = false;
		}


		private void OnCollisionEnter2D(Collision2D other)
		{
			if (!isRolling) return;
			if (other.gameObject.tag.Contains("Enemy")) { // kill off other enemies
				Enemy enemy = other.gameObject.GetComponent<Enemy>();
				_levelManager.GetPlayerAbilities.RollingShellTouchEnemy (enemy);
			} else {
				_currentRollVelocityX = -_currentRollVelocityX;
			}
		}
	}
}
