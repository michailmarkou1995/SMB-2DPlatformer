using System.Collections;
using Abilities.NPC;
using Core.Managers;
using UnityEngine;
using IPlayerController = Core.Player.PlayerController;

namespace Core.NPC
{
	public class Bowser : Enemy {
		private LevelManager _levelManager;
		private GameObject _mario;
		private Rigidbody2D _rigidbody2D;

		public Transform firePos;
		public GameObject bowserImpostor;
		public GameObject bowserFire;
		public bool canMove;
		public bool active;

		private Vector2 impostorInitialVelocity = new Vector2 (3, 3);
		private float minDistanceToMove = 55; // start moving if mario is within this distance

		private int _fireResistance = 5;
		private float waitBetweenJump = 3;
		private float shootFireDelay = .1f; // how long after jump should Bowser release fireball

		private float absSpeedX = 1.5f;
		private float _directionX = 1;
		private float minJumpSpeedY = 3;
		private float maxJumpSpeedY = 7;

		private float _timer;
		private float _jumpSpeedY;

		private int _defeatBonus;
		private bool _isFalling;

		// Use this for initialization
		private void Start () {
			_levelManager = FindObjectOfType<LevelManager> ();
			_mario = FindObjectOfType<IPlayerController> ().gameObject;
			_rigidbody2D = GetComponent<Rigidbody2D> ();
			_timer = 0;
			canMove = false;
			active = true;

			starmanBonus = 0; 
			rollingShellBonus = 0;
			hitByBlockBonus = 0;
			fireballBonus = 0;
			stompBonus = 0;
			_defeatBonus = 5000;
		}
	
		// Update is called once per frame
		private void Update () {
			if (active) {
				if (!canMove && Mathf.Abs (_mario.gameObject.transform.position.x - transform.position.x) <= minDistanceToMove) {
					canMove = true;
				}

				if (canMove) {
					_rigidbody2D.velocity = new Vector2 (_directionX * absSpeedX, _rigidbody2D.velocity.y);
					_timer -= Time.deltaTime;

					if (_timer <= 0) {
						// Turn to face Mario
						if (_mario.transform.position.x < transform.position.x) { // mario to the left
							transform.localScale = new Vector3 (-1, 1, 1);
						} else if (_mario.transform.position.x > transform.position.x) {
							transform.localScale = new Vector3 (1, 1, 1);
						}

						// Switch walk direction
						_directionX = -_directionX;

						// Jump a random height
						_jumpSpeedY = Random.Range (minJumpSpeedY, maxJumpSpeedY);
						_rigidbody2D.velocity = new Vector2 (_rigidbody2D.velocity.x, _jumpSpeedY);

						// Shoot fireball after some delay
						StartCoroutine (ShootFireCo (shootFireDelay));

						_timer = waitBetweenJump;
					}

				}
			} else if (_rigidbody2D.velocity.y < 0 && !_isFalling) { // fall as bridge collapses
				_isFalling = true;
				_levelManager.GetSoundManager.SoundSource.PlayOneShot (_levelManager.GetSoundManager.BowserFallSound);
			}
		}

		private IEnumerator ShootFireCo(float delay) {
			yield return new WaitForSeconds (delay);
			GameObject fire = Instantiate(bowserFire, firePos.position, Quaternion.identity);
			fire.GetComponent<BowserFire> ().directionX = transform.localScale.x;
			_levelManager.GetSoundManager.SoundSource.PlayOneShot (_levelManager.GetSoundManager.BowserFireSound);
		}

		public override void TouchedByStarmanMario() {
		}

		public override void TouchedByRollingShell() {
		}

		public override void HitBelowByBlock() {
		}

		public override void HitByMarioFireball() {
			_fireResistance--;
			if (_fireResistance > 0) return;
			GameObject impostor = Instantiate (bowserImpostor, transform.position, Quaternion.identity);
			impostor.GetComponent<Rigidbody2D> ().velocity = 
				new Vector2 (impostorInitialVelocity.x * _directionX, impostorInitialVelocity.y);
			_levelManager.GetSoundManager.SoundSource.PlayOneShot (_levelManager.GetSoundManager.BowserFallSound);

			_levelManager.GetPlayerPickUpAbilities.AddScore (_defeatBonus);
			Destroy (gameObject);
		}

		public override void StompedByMario() {
		}

		private void OnCollisionEnter2D(Collision2D other) {
			Vector2 normal = other.contacts[0].normal;
			Vector2 leftSide = new Vector2 (-1f, 0f);
			Vector2 rightSide = new Vector2 (1f, 0f);
			bool sideHit = normal == leftSide || normal == rightSide;

			if (other.gameObject.CompareTag("Player")) {
				_levelManager.GetPlayerAbilities.MarioPowerDown ();
			} else if (sideHit && !other.gameObject.CompareTag("Mario Fireball")) { // switch walk direction
				_directionX = -_directionX;
			}
		}
	}
}
