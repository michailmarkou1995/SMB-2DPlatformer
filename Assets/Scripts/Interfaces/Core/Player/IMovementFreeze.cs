using System;

namespace Interfaces.Core.Player
{
    public interface IMovementFreeze
    {
        public event Action InputFreeze;
        public bool InputFreezed { get; set; }
        public void FreezeUserInput();
        public void UnfreezeUserInput();
        public void FreezeAndDie();
    }
}