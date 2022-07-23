using Core.Managers;
using Core.NPC;
using UnityEngine;

namespace Abilities.Player
{
	public class MarioStompBox : MonoBehaviour {
		private LevelManager tLevelManager;

		// Use this for initialization
		private void Start () {
			tLevelManager = FindObjectOfType<LevelManager> ();
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if (!other.gameObject.tag.Contains("Enemy") || other.gameObject.CompareTag("Enemy/Piranha") ||
			    other.gameObject.CompareTag("Enemy/Bowser")) return;
			Debug.Log (this.name + " OnTriggerEnter2D: recognizes " + other.gameObject.name);
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			tLevelManager.MarioStompEnemy (enemy);
			Debug.Log (this.name + " OnTriggerEnter2D: finishes calling stomp method on " + other.gameObject.name);
		}
	}
}
