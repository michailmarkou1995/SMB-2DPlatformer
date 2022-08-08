using System;
using UnityEngine;

namespace Abilities.Pickups
{
    public class OneUpMushroom : MonoBehaviour, ICollectible
    {
        private Rigidbody2D _rigidbody2D;
        public Vector2 initialVelocity;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.velocity = initialVelocity;
        }

        #region ActionEvent

        // Callback signature
        // Returns void and requests a Vector3
        //public delegate void AnswerCallback(Vector3 spawnPos);

        // Event declaration
        //public static event AnswerCallback OnOneUpCollected;

        public static event Action<Vector3> OnOneUpCollected = delegate(Vector3 spawnPos) { };

        #endregion

        // Calls, "raises" or "invokes" the event. Note that if no one subscribed
        // to the event, the event will be null. We need to check this first to prevent errors.
        public void Collect()
        {
            if (OnOneUpCollected == null) return;
            GameObject o = gameObject;
            OnOneUpCollected(o.transform.position + Vector3.up * 2);
            Destroy(o);
        }
    }
}