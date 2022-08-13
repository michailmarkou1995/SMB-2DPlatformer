using Core.Managers;
using Core.NPC;
using UnityEngine;

namespace Interfaces.Core.NPC
{
    public interface IBowser
    {
        public Bowser BowserSelf { get; }
        public LevelManager LevelManager { get; set; }
        public GameObject Mario { get; set; }
        public Rigidbody2D Rigidbody2D { get; set; }

        public Transform FirePos { get; set; }
        public GameObject BowserImpostor { get; set; }
        public GameObject BowserFire { get; set; }
        public bool CanMove { get; set; }
        public bool Active { get; set; }

        public Vector2 ImpostorInitialVelocity { get; set; }
        public float MinDistanceToMove { get; set; }

        public int FireResistance { get; set; }
        public float WaitBetweenJump { get; set; }
        public float ShootFireDelay { get; set; }

        public float AbsSpeedX { get; set; }
        public float DirectionX { get; set; }
        public float MinJumpSpeedY { get; set; }
        public float MaxJumpSpeedY { get; set; }

        public float Timer { get; set; }
        public float JumpSpeedY { get; set; }

        public int DefeatBonus { get; set; }
        public bool IsFalling { get; set; }

        //METHODS
        void ShootFire(float delay = 0);
        void BowserSetup();
    }
}