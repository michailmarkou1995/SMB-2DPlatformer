using System;
using UnityEngine;

namespace Pickups {
    public class Coin : MonoBehaviour, ICollectible
    {
        public static event Action OnCoinCollected;
        public void Collect()
        {
            Destroy(gameObject);
            OnCoinCollected?.Invoke();
        }

    }
}
