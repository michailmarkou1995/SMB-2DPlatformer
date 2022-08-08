using Core.Managers;
using Core.NPC;
using UnityEngine;
using IPlayerController = Core.Player.PlayerController;

namespace Level
{
	public class Firebar : Enemy {
		public Transform pivot;
		public float rotateSpeed = 75;
		private LevelManager _levelManager;
		private GameObject player;
		public bool canMove;
		private bool canMoveAutomatic = true;
		private float minDistanceToMove = 14f;

		private void Start () {
			_levelManager = FindObjectOfType<LevelManager> ();
			player = FindObjectOfType<IPlayerController> ().gameObject;

			starmanBonus = 0;
			rollingShellBonus = 0;
			hitByBlockBonus = 0;
			fireballBonus = 0;
			stompBonus = 0;
		}

		private void Update() {
			if (!canMove & Mathf.Abs (player.transform.position.x - transform.position.x) <= minDistanceToMove && canMoveAutomatic) {
				canMove = true;
			} else if (canMove) {
				transform.RotateAround(pivot.position, Vector3.forward, rotateSpeed * Time.deltaTime);
			}
		}

		public override void TouchedByStarmanMario() {
		}

		public override void TouchedByRollingShell() {
		}

		public override void HitBelowByBlock() {
		}

		public override void HitByMarioFireball() {
		}

		public override void StompedByMario() {
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if (other.CompareTag("Player")) {
				_levelManager.GetPlayerAbilities.MarioPowerDown ();
			}
		}

	}
}
