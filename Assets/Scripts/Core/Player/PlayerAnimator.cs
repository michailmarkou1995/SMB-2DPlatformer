using UnityEngine;

namespace Core.Player
{
    public static class PlayerAnimator
    {
        public static Animator PlayerAnimatorComponent;

        public static readonly int IsInvincibleStarmanAnim = Animator.StringToHash("isInvincibleStarman");
        public static readonly int IsInvinciblePowerdownAnim = Animator.StringToHash("isInvinciblePowerdown");
        public static readonly int IsPoweringUpAnim = Animator.StringToHash("isPoweringUp");
        public static readonly int IsPoweringDownAnim = Animator.StringToHash("isPoweringDown");
    }
}