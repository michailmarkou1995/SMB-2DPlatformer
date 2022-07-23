using System;
using UnityEngine;

namespace Abilities.Pickups {
    public class Starman : MonoBehaviour, ICollectible
    {
        // Event declaration
        public static event Action OnStarmanCollected;
        public void Collect()
        {
            OnStarmanCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
