using System.Collections.Generic;
using Core.Managers;
using Core.NPC;
using UnityEngine;


/* Spawn object if bumped by Player's head
 * Applicable to: Collectible brick and question blocks
 */

namespace Abilities.Pickups {
	public class CollectibleBlock : MonoBehaviour {
		private Animator _animator;
		private LevelManager _levelManager;

		public bool isPowerupBlock;
		public GameObject objectToSpawn;
		public GameObject bigMushroom;
		public GameObject fireFlower;
		public int timesToSpawn = 1;
		public Vector3 spawnPositionOffset;

		private float WaitBetweenBounce = .25f;
		private bool _isActive;
		private float _time1, _time2;

		public List<GameObject> enemiesOnTop = new List<GameObject> ();

		// Use this for initialization
		private void Start () {
			_animator = GetComponent<Animator> ();
			_levelManager = FindObjectOfType<LevelManager> ();
			_time1 = Time.time;
			_isActive = true;
		}

		void OnTriggerEnter2D(Collider2D other) {
			_time2 = Time.time;
			if (!other.CompareTag("Player") || !(_time2 - _time1 >= WaitBetweenBounce)) return;
			_levelManager.GetSoundManager.SoundSource.PlayOneShot (_levelManager.GetSoundManager.BumpSound);

			if (_isActive) {
				_animator.SetTrigger ("bounce");

				// Hit any enemy on top
				foreach (GameObject enemyObj in enemiesOnTop) {
					_levelManager.GetPlayerAbilities.BlockHitEnemy (enemyObj.GetComponent<Enemy> ());
				}

				if (timesToSpawn > 0) {
					if (isPowerupBlock) { // spawn mushroom or fireflower depending on Mario's size
						if (_levelManager.GetGameStateData.PlayerSize == 0) {
							objectToSpawn = bigMushroom;
						} else {
							objectToSpawn = fireFlower;
						}
					}
					Instantiate (objectToSpawn, transform.position + spawnPositionOffset, Quaternion.identity);
					timesToSpawn--;

					if (timesToSpawn == 0) {
						_animator.SetTrigger ("deactivated");
						_isActive = false;
					}			
				}
			}

			_time1 = Time.time;
		}


		// check for enemy on top
		void OnCollisionStay2D(Collision2D other) {
			Vector2 normal = other.contacts[0].normal;
			Vector2 topSide = new Vector2 (0f, -1f);
			bool topHit = normal == topSide;
			if (other.gameObject.tag.Contains("Enemy") && topHit && !enemiesOnTop.Contains(other.gameObject)) {
				enemiesOnTop.Add(other.gameObject);
			}
		}

		void OnCollisionExit2D(Collision2D other) {
			if (other.gameObject.tag.Contains("Enemy")) {
				enemiesOnTop.Remove (other.gameObject);
			}
		}
	}
}
