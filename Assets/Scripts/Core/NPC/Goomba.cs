using UnityEngine;

namespace Core.NPC
{
    public class Goomba : Enemy {
        private Animator _animator;
        private const float stompedDuration = 0.5f;
        private static readonly int Stomped = Animator.StringToHash("stomped");

        private void Start() {
            _animator = GetComponent<Animator>();

            starmanBonus = 100;
            rollingShellBonus = 500;
            hitByBlockBonus = 100;
            fireballBonus = 100;
            stompBonus = 100;
        }

        public override void StompedByMario() {
            isBeingStomped = true;
            StopInteraction();
            Debug.Log(this.name + " StompedByMario: stopped interaction");
            _animator.SetTrigger(Stomped);
            Destroy(gameObject, stompedDuration);
            isBeingStomped = false;
        }
    }
}