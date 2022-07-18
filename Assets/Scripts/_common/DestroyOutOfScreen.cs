using UnityEngine;

/* Destroy if object goes out of screen
 * Applicable to: Vertical Moving Platform for Spawner, Mario Fireball
 */

namespace _common {
    public class DestroyOutOfScreen : MonoBehaviour {
        private void OnBecameInvisible() {
            Destroy(gameObject);
        }
    }
}