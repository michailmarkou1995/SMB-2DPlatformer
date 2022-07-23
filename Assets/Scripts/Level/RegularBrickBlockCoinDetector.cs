using UnityEngine;

namespace Level
{
	public class RegularBrickBlockCoinDetector : MonoBehaviour {
		public GameObject coinOnTop;

		void OnTriggerStay2D(Collider2D other) {
			if (other.tag == "Coin") {
				coinOnTop = other.gameObject;
			}
		}
	}
}
