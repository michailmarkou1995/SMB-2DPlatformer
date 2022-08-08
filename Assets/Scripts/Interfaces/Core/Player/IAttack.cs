using UnityEngine.InputSystem;

namespace Interfaces.Core.Player
{
    public interface IAttack
    {
        public bool IsShooting { get; set; }
        public void Shooting(InputAction.CallbackContext obj);
        public float FireTime1 { get; set; }
        public float FireTime2 { get; set; }
    }
}