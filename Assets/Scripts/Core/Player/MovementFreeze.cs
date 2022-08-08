using System;
using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class MovementFreeze : MonoBehaviour, IMovementFreeze
    {
        public event Action InputFreeze;
        public bool InputFreezed { get; set; }

        private IPlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<IPlayerController>();
        }

        protected virtual void OnInputFreeze()
        {
            InputFreeze?.Invoke();
        }

        public void FreezeUserInput()
        {
            _playerController.PlayerControls.Player.Disable();
            InputFreezed = true;
            OnInputFreeze();
            _playerController.GetJump.JumpButtonHeld = false;

            _playerController.GetMovement.FaceDirectionX = 0;
            _playerController.GetMovement.MoveDirectionX = 0;

            _playerController.GetMovement.CurrentSpeedX = 0;
            _playerController.GetMovement.SpeedXBeforeJump = 0;
            _playerController.GetMovement.AutomaticWalkSpeedX = 0;
            _playerController.GetJump.AutomaticGravity = _playerController.GetJump.NormalGravity;

            _playerController.GetDash.IsDashing = false;
            _playerController.GetDash.WasDashingBeforeJump = false;
            _playerController.GetCrouch.IsCrouching = false;
            _playerController.GetMovement.IsChangingDirection = false;
            _playerController.GetAttack.IsShooting = false;

            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // stop all momentum
        }

        public void UnfreezeUserInput()
        {
            _playerController.PlayerControls.Player.Enable();
            InputFreezed = false;
        }

        public void FreezeAndDie()
        {
            // TODO extract logic e.g., respawn to its own class search every other class for same stuff
            // TODO Check every single line if can be replaced with function call ***
            FreezeUserInput();
            _playerController.GetDeath.IsDying = true;
            _playerController.MRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            PlayerAnimatorStatic.PlayerAnimatorComponent.SetTrigger(PlayerAnimatorStatic.RespawnAnimator);
            gameObject.layer = LayerMask.NameToLayer("Falling to Kill Plane");
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground Effect";
        }
    }
}