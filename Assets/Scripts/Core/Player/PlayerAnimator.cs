using System;
using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class PlayerAnimator : MonoBehaviour, IPlayerAnimator
    {
        public Animator PlayerAnimatorComponent { get; set; }

        private void Awake()
        {
            PlayerAnimatorComponent = GetComponent<Animator>();
        }

        // Exposed to Editor explicit backing fields of properties Form
        [Header("Animator Name Parameters")] [Tooltip("Default value: playerSize")] [SerializeField]
        private string playerSizeAnimator = "playerSize";

        [Tooltip("Default value: climbFlagPole")] [SerializeField]
        private string poleAnimator = "climbFlagPole";

        [Tooltip("Default value: respawn")] [SerializeField]
        private string respawnAnimator = "respawn";

        [Tooltip("Default value: isJumping")] [SerializeField]
        private string isJumpingAnimator = "isJumping";

        [Tooltip("Default value: isFallingNotFromJump")] [SerializeField]
        private string isFallingNotFromJumpAnimator = "isFallingNotFromJump";

        [Tooltip("Default value: isCrouching")] [SerializeField]
        private string isCrouchingAnimator = "isCrouching";

        [Tooltip("Default value: absSpeed")] [SerializeField]
        private string absSpeedAnimator = "absSpeed";

        [Tooltip("Default value: isFiring")] [SerializeField]
        private string isFiringAnimator = "isFiring";

        [Tooltip("Default value: isSkidding")] [SerializeField]
        private string isSkiddingAnimator = "isSkidding";

        [Tooltip("Default value: isInvincibleStarman")] [SerializeField]
        private string isInvincibleStarman = "isInvincibleStarman";

        [Tooltip("Default value: isInvinciblePowerdown")] [SerializeField]
        private string isInvinciblePowerdown = "isInvinciblePowerdown";

        [Tooltip("Default value: isPoweringUp")] [SerializeField]
        private string isPoweringUp = "isPoweringUp";

        [Tooltip("Default value: isPoweringDown")] [SerializeField]
        private string isPoweringDown = "isPoweringDown";


        // getters for exposed backing fields
        public int PlayerSizeAnimator => Animator.StringToHash(playerSizeAnimator);

        public int PoleAnimator => Animator.StringToHash(poleAnimator);

        public int RespawnAnimator => Animator.StringToHash(respawnAnimator);

        public int IsJumpingAnimator => Animator.StringToHash(isJumpingAnimator);

        public int IsFallingNotFromJumpAnimator => Animator.StringToHash(isFallingNotFromJumpAnimator);

        public int IsCrouchingAnimator => Animator.StringToHash(isCrouchingAnimator);

        public int AbsSpeedAnimator => Animator.StringToHash(absSpeedAnimator);

        public int IsFiringAnimator => Animator.StringToHash(isFiringAnimator);

        public int IsSkiddingAnimator => Animator.StringToHash(isSkiddingAnimator);

        public int IsInvincibleStarman => Animator.StringToHash(isInvincibleStarman);

        public int IsInvinciblePowerdown => Animator.StringToHash(isInvinciblePowerdown);

        public int IsPoweringUp => Animator.StringToHash(isPoweringUp);

        public int IsPoweringDown => Animator.StringToHash(isPoweringDown);
    }
}