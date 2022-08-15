using Core.Player;
using Interfaces.Core;
using UnityEngine;

namespace UI
{
    /// <summary>
    ///     Destroy if object goes out of screen<br />
    ///     Applicable to:<br />
    ///     <list type="bullet">
    ///         <item>
    ///             <b>Vertical Moving Platform for Spawner</b>
    ///         </item>
    ///         <item>
    ///             <b>Mario Fireball</b>
    ///         </item>
    ///     </list>
    /// </summary>
    public class DestroyOutOfScreen : MonoBehaviour, IDestroy
    {
        private IDestroy _destroyImplementation;

        private void OnBecameInvisible()
        {
            DestroyGameObj(gameObject);
        }

        public void DestroyGameObj(GameObject gameObject, float delay = 0)
        {
            Destroy(gameObject);
        }
    }
}