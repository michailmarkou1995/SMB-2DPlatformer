using UnityEngine;

namespace Abilities.Pickups
{
    public class Collector : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            ICollectible collectible = collision.GetComponent<ICollectible>();
            collectible?.Collect();
        }
    }
}