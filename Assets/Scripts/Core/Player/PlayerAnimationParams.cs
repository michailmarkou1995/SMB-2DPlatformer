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

        public void AnimationParams()
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
    }
}