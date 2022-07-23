using System;
using UnityEngine;

namespace Abilities.Pickups {
    public class Coin : MonoBehaviour, ICollectible
    {
        // Event declaration
        public static event Action OnCoinCollected;
        public void Collect()
        {
            OnCoinCollected?.Invoke();
            Destroy(gameObject);
        }

    }
}
