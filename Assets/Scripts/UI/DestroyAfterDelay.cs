using UnityEngine;

namespace UI
{
    /// <summary>
    /// Automatically destroy game object after delay<br/>
    /// Applicable to:<br/>
    /// <list type="bullet">
    /// <item><b>Mario Fireball</b></item>
    /// <item><b>Brick Block Temp Collider</b></item>
    /// </list>
    /// </summary>
    public class DestroyAfterDelay : MonoBehaviour
    {
        [SerializeField] private float delay;

        private void Start()
        {
            //if (delay == 0) Debug.LogWarning("0 delay");
            Destroy(gameObject, delay);
        }
    }
}