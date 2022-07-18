using UnityEngine;

/* Automatically destroy game object after delay
 * Applicable to: Mario Fireball, Brick Block Temp Collider
 */

namespace _common {
    public class DestroyAfterDelay : MonoBehaviour {
        public float delay;

        // Use this for initialization
        private void Start() {
            Destroy(gameObject, delay);
        }
    }
}