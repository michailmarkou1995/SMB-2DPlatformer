using UnityEngine;

namespace Interfaces.Abilities.Player
{
    public abstract class PlayerAbilitiesBase : MonoBehaviour
    {
        protected Rigidbody2D _marioRigidbody2D;
        protected bool _isRespawning;
        [SerializeField] protected Vector2 stompBounceVelocity = new(0, 15);
        [SerializeField] protected bool isPoweringDown;
        [SerializeField] protected bool isInvinciblePowerdown;
        [SerializeField] protected bool isInvincibleStarman;

        protected const float MarioInvinciblePowerdownDuration = 2f;
        protected const float MarioInvincibleStarmanDuration = 12f;
        protected const float TransformDuration = 1f;
    }
}