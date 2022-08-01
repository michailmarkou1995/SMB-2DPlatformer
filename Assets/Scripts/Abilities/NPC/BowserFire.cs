using Core.Managers;
using Core.NPC;
using UnityEngine;

namespace Abilities.NPC
{
	public class BowserFire : Enemy {
		private LevelManager _levelManager;
		private Rigidbody2D _rigidbody2D;

		private float absSpeedX = 18;
		public float directionX = -1; // 1 for right, -1 for left

		void Start () {
			_levelManager = FindObjectOfType<LevelManager> ();
			_rigidbody2D = FindObjectOfType<Rigidbody2D> ();
			transform.localScale = new Vector3 (directionX, 1, 1); // orient sprite

			starmanBonus = 0;
			rollingShellBonus = 0;
			hitByBlockBonus = 0;
			fireballBonus = 0;
			stompBonus = 0;
		}

		private void Update() {
			_rigidbody2D.velocity = new Vector2 (absSpeedX * directionX, _rigidbody2D.velocity.y);
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
