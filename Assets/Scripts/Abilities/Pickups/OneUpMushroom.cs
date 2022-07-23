using UnityEngine;

namespace Abilities.Pickups
{
    public class OneUpMushroom : MonoBehaviour, ICollectible
    {
        private Rigidbody2D m_Rigidbody2D;
        public Vector2 initialVelocity;

        // Use this for initialization
        void Start()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_Rigidbody2D.velocity = initialVelocity;
        }

        // Callback signature
        // Returns void and requests a Vector3
        public delegate void AnswerCallback(Vector3 spawnPos);

        // Event declaration
        public static event AnswerCallback OnOneUpCollected;

        // Calls, "raises" or "invokes" the event. Note that if no one subscribed
        // to the event, the event will be null. We need to check this first to prevent errors.
        public void Collect()
        {
            if (OnOneUpCollected != null)
            {
                OnOneUpCollected(gameObject.transform.position + Vector3.up * 2);
                Destroy(gameObject);
            }
        }
    }
}
