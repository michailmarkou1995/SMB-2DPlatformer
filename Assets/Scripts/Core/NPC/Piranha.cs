using Abilities.NPC;
using Core.Managers;
using Core.Player;
using UnityEngine;

namespace Core.NPC
{
	public class Piranha : Enemy {
		private LevelManager _levelManager;
		[SerializeField] private GameObject mario;
		private CircleCollider2D _circleCollider2D;
		private PatrolVertical _patrolScript;

		private bool _visible;
		private float maxDistanceToMove = 2; // should not emerge if Mario is within this distance of pipe

		private void Start () {
			_levelManager = FindObjectOfType<LevelManager> ();
			mario = FindObjectOfType<PlayerController> ().gameObject;
			_circleCollider2D = GetComponent<CircleCollider2D> ();
			_patrolScript = GetComponent<PatrolVertical> ();
			_visible = false;
			_patrolScript.canMove = false;
			_circleCollider2D.enabled = false;

			starmanBonus = 100; // ???
			rollingShellBonus = 500; // ???
			hitByBlockBonus = 0;
			fireballBonus = 200;
			stompBonus = 0;
		}

		private void OnBecameVisible() {
			_visible = true;
		}

		private void Update()
		{
			if (!_visible) return;
			if (Mathf.Abs (mario.transform.position.x - transform.position.x) > maxDistanceToMove) {
				_circleCollider2D.enabled = true;
				_patrolScript.canMove = true;
			} else if (_patrolScript.isAtDownStop) { // do not emerge
				_circleCollider2D.enabled = false;
				_patrolScript.canMove = false;
			}
		}

		private void DestroyPiranhaStruct() {
			Destroy (gameObject.transform.parent.gameObject);
		}

		public override void TouchedByStarmanMario() {
			DestroyPiranhaStruct ();
		}

		public override void TouchedByRollingShell() {
			DestroyPiranhaStruct ();
		}

		public override void HitBelowByBlock() {
		}

		public override void HitByMarioFireball() {
			DestroyPiranhaStruct ();
		}

		public override void StompedByMario() {
		}

		void OnTriggerEnter2D(Collider2D other) {
			if (other.tag == "Player") {
				_levelManager.GetPlayerAbilities.MarioPowerDown ();
			}
		}
	}
}
