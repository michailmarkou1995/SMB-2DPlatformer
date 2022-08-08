using Abilities.NPC;
using UnityEngine;
using IPlayerController = Core.Player.PlayerController;

namespace Level
{
	public class MovingPlatformVerticalSpawner : MonoBehaviour {
		public GameObject MovingPlatform;
		public bool isMoving;
		public float directionY = 1; // 1 for up, -1 for down 

		public Transform UpStop;
		public Transform DownStop;
		public Transform SpawnPos;

		private const float WaitBetweenSpawn = 1.5f;
		private readonly float minDistanceToMove = 40; // how close should Mario be for platforms to appear

		private GameObject _player;
		private float _timer;

		private void Start () {
			_player = FindObjectOfType<IPlayerController> ().gameObject;
			_timer = WaitBetweenSpawn / 2;
			isMoving = false;
		}


		// Update is called once per frame
		private void Update ()
		{
			isMoving = Mathf.Abs (_player.transform.position.x - transform.position.x) <= minDistanceToMove;

			if (!isMoving) return;
			_timer -= Time.deltaTime;

			if (!(_timer <= 0)) return;
			GameObject clone = Instantiate (MovingPlatform, SpawnPos.position, Quaternion.identity);
			PatrolVertical patrolScript = clone.GetComponent<PatrolVertical> ();
			patrolScript.UpStop = UpStop;
			patrolScript.DownStop = DownStop;
			patrolScript.directionY = directionY;
			patrolScript.canMove = true;
			_timer = WaitBetweenSpawn;
		}
	}
}
