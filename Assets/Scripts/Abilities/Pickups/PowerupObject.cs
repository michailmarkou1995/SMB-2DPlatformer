using System;
using UnityEngine;

/* Powerup collectible behaviors
 * Applicable to: Big Mushroom, Fireflower
 */

namespace Abilities.Pickups
{
    public class PowerupObject : MonoBehaviour, ICollectible
    {
        private Rigidbody2D m_Rigidbody2D;
        public Vector2 initialVelocity;

        // Use this for initialization
        void Start()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_Rigidbody2D.velocity = initialVelocity;
        }

        // Event declaration
        public static event Action OnPowerUpCollected;

        public void Collect()
        {
            OnPowerUpCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
