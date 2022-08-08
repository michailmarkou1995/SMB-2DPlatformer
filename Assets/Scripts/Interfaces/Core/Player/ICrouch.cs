using UnityEngine.InputSystem;

namespace Interfaces.Core.Player
{
    public interface ICrouch
    {
        public void Crouch_performed(InputAction.CallbackContext context);
        public void Crouch_canceled(InputAction.CallbackContext context);
        public bool IsCrouching { get; set; }
        public void AutomaticCrouch();
    }
}