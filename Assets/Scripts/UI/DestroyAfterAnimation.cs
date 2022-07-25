using Interfaces.Core;
using UnityEngine;

namespace UI
{
    /// <summary>
    ///     Automatically delete game object after animation finishes<br />
    ///     Applicable to:<br />
    ///     <list type="bullet">
    ///         <item>
    ///             <b>Block Coin</b>
    ///         </item>
    ///         <item>
    ///             <b>Floating Text Effect</b>
    ///         </item>
    ///     </list>
    /// </summary>
    public class DestroyAfterAnimation : MonoBehaviour, IDestroyAfterAnimation
    {
        [SerializeField] private float delay; // optional delay before destroying

        private void Start()
        {
            DestroyAfterAnim();
        }

        public void DestroyAfterAnim()
        {
            //if (delay == 0) Debug.LogWarning("0 delay");
            if (gameObject.transform.parent.gameObject)
                Destroy(gameObject.transform.parent.gameObject,
                    GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);

            Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
        }
    }
}