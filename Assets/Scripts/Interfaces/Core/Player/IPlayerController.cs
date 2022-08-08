using System;
using Interfaces.Core.Managers;
using UnityEngine;

namespace Interfaces.Core.Player
{
    public interface IPlayerController
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        public GameObject MStompBox { get; set; }
        public Rigidbody2D MRigidbody2D { get; set; }
        public CapsuleCollider2D MCapsuleCollider2D { get; set; }
        public BoxCollider2D MBoxCollider2D { get; set; }
        public Transform MGroundCheck1 { get; set; }

        public Transform MGroundCheck2 { get; set; }
        public Collider2D[] Colliders1 { get; set; }
        public Collider2D[] Colliders2 { get; set; }

        //public IPlayerAnimator GetPlayerAnimator { get; }
        public PlayerInputActions PlayerControls { get; set; }
        public ILevelManager GetLevelManager { get; }
        public ICrouch GetCrouch { get; }
        public IMove GetMovement { get; }
        public IJump GetJump { get; }
        public IDash GetDash { get; }
        public IAttack GetAttack { get; }
        public IGroundCheck GetGroundCheck { get; }
        public IMovementFreeze GetMovementFreeze { get; }
        public IPlayerAnimationParams GetAnimationParams { get; }
        public IPlayerSize GetPlayerSize { get; }
        public IDeath GetDeath { get; }
        public void PlayerControlsSubscribe() { }
        public void InitializationComponents() { }

        // OLD IMPLEMENTATION DEFAULT BEGINS HERE ... MIGRATION COMPABILITY
        [Obsolete("UpdateSize is deprecated, please use IPlayerSize instead.", true)]
        public void UpdateSize()
        {
            throw new NotImplementedException();
        }
    }
}