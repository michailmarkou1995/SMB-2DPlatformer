using Interfaces.Core;
using UnityEngine;

namespace UI
{
    /// <summary>
    ///     Automatically destroy game object after delay<br />
    ///     Applicable to:<br />
    ///     <list type="bullet">
    ///         <item>
    ///             <b>Mario Fireball</b>
    ///         </item>
    ///         <item>
    ///             <b>Brick Block Temp Collider</b>
    ///         </item>
    ///     </list>
    /// </summary>
    public class DestroyAfterDelay : MonoBehaviour, IDestroy
    {
        [SerializeField] private float delay;

        private void Start()
        {
            DestroyGameObj(gameObject, delay);
        }

        public void DestroyGameObj(GameObject gameObject, float delay)
        {
            //if (delay == 0) Debug.LogWarning("0 delay");
            Destroy(gameObject, delay);
        }
    }
}