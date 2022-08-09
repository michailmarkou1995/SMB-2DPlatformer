using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class PlayerAnimationParams : MonoBehaviour, IPlayerAnimationParams
    {
        private IPlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<IPlayerController>();
        }

        public void MovementAnimationParams()
        {
            PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(
                PlayerAnimatorStatic.IsJumpingAnimator, _playerController.GetJump.IsJumping);
            PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(
                PlayerAnimatorStatic.IsFallingNotFromJumpAnimator,
                _playerController.GetJump.IsFalling && !_playerController.GetJump.IsJumping);
            PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(
                PlayerAnimatorStatic.IsCrouchingAnimator, _playerController.GetCrouch.IsCrouching);
            PlayerAnimatorStatic.PlayerAnimatorComponent.SetFloat(
                PlayerAnimatorStatic.AbsSpeedAnimator, Mathf.Abs(_playerController.GetMovement.CurrentSpeedX));
        }

        public void AnimPowerUp(int animName, bool isPowerUp)
        {
            PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(animName, isPowerUp);
        }

        // public void InvinciblePowerdown_ON()
        // {
        //     PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(PlayerAnimatorStatic.IsInvinciblePowerdownAnim, true);
        // }
        // public void InvinciblePowerdown_OFF()
        // {
        //     PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(PlayerAnimatorStatic.IsInvinciblePowerdownAnim, false);
        // }
        // public void InvincibleStarman_ON()
        // {
        //     PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(PlayerAnimatorStatic.IsInvincibleStarmanAnim, true);
        // }
        // public void InvincibleStarman_OFF()
        // {
        //     PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(PlayerAnimatorStatic.IsInvincibleStarmanAnim, false);
        // }
    }
}