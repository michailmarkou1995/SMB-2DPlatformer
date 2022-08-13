using Core.NPC;
using Core.Managers;
using UnityEngine;

namespace Interfaces.Core.NPC
{
	public abstract class BowserBase : Enemy
	{
		protected LevelManager LevelManager;
		protected GameObject Player;
		protected Rigidbody2D Rb2D;

		[SerializeField] protected Transform firePos;
		[SerializeField] protected GameObject bowserImpostor;
		[SerializeField] protected GameObject bowserFire;
		protected bool CanMove;
		protected bool Active;

		protected Vector2 ImpostorInitialVelocity = new(3, 3);
		protected float MinDistanceToMove = 55; // start moving if mario is within this distance

		protected int FireResistance = 5;
		protected float WaitBetweenJump = 3;
		protected float ShootFireDelay = .1f; // how long after jump should Bowser release fireball

		protected float AbsSpeedX = 1.5f;
		protected float DirectionX = 1;
		protected float MinJumpSpeedY = 3;
		protected float MaxJumpSpeedY = 7;

		protected float Timer;
		protected float JumpSpeedY;

		protected int DefeatBonus;
		protected bool IsFalling;
	}
}
