using UnityEngine;

namespace UI
{
    /// <summary>
    /// Destroy if object goes out of screen<br/>
    /// Applicable to:<br/>
    /// <list type="bullet">
    /// <item><b>Vertical Moving Platform for Spawner</b></item>
    /// <item><b>Mario Fireball</b></item>
    /// </list>
    /// </summary>
    public class DestroyOutOfScreen : MonoBehaviour
    {
        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}