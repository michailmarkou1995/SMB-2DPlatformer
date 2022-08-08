using Interfaces.Core.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    public class Crouch : MonoBehaviour, ICrouch
    {
        public bool IsCrouching { get; set; }

        private IPlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public void Crouch_performed(InputAction.CallbackContext context)
        {
            if (_playerController.GetMovementFreeze.InputFreezed) return;
            IsCrouching = true;
            _playerController.PlayerControls.Player.Move.Disable();
            _playerController.GetMovement.CurrentSpeedX = 0;
        }

        public void Crouch_canceled(InputAction.CallbackContext context)
        {
            if (_playerController.GetMovementFreeze.InputFreezed) return;
            IsCrouching = false;
            _playerController.PlayerControls.Player.Move.Enable();
        }

        public void AutomaticCrouch()
        {
            _playerController.GetMovementFreeze.FreezeUserInput();
            IsCrouching = true;
        }
    }
}