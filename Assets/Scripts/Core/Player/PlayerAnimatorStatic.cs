using UnityEngine;

namespace Core.Player
{
    public static class PlayerAnimatorStatic
    {
        public static Animator PlayerAnimatorComponent;
    
        public static readonly int PlayerSizeAnimator = Animator.StringToHash("playerSize");
        public static readonly int PoleAnimator = Animator.StringToHash("climbFlagPole");
        public static readonly int RespawnAnimator = Animator.StringToHash("respawn");
        public static readonly int IsJumpingAnimator = Animator.StringToHash("isJumping");
        public static readonly int IsFallingNotFromJumpAnimator = Animator.StringToHash("isFallingNotFromJump");
        public static readonly int IsCrouchingAnimator = Animator.StringToHash("isCrouching");
        public static readonly int AbsSpeedAnimator = Animator.StringToHash("absSpeed");
        public static readonly int IsFiringAnimator = Animator.StringToHash("isFiring");
        public static readonly int IsSkiddingAnimator = Animator.StringToHash("isSkidding");
        public static readonly int IsInvincibleStarmanAnim = Animator.StringToHash("isInvincibleStarman");
        public static readonly int IsInvinciblePowerdownAnim = Animator.StringToHash("isInvinciblePowerdown");
        public static readonly int IsPoweringUpAnim = Animator.StringToHash("isPoweringUp");
        public static readonly int IsPoweringDownAnim = Animator.StringToHash("isPoweringDown");
    }
}