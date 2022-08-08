using UnityEngine.InputSystem;

namespace Interfaces.Core.Player
{
    public interface IJump
    {
        public bool IsFalling { get; set; }
        public bool IsJumping { get; set; }
        public bool IsChangedDirOnAirYes { get; set; }
        public bool JumpButtonHeld { get; set; }
        public float AutomaticGravity { get; set; }
        public float NormalGravity { get; set; }
        public float JumpSpeedY { get; set; }
        public float JumpUpGravity { get; set; }
        public float JumpDownGravity { get; set; }
        public void Jump_performed(InputAction.CallbackContext context);
        public void Jump_canceled(InputAction.CallbackContext context);
        public void JumpOffPole();
    }
}