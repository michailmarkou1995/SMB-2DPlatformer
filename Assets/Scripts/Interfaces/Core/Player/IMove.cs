﻿using UnityEngine;

namespace Interfaces.Core.Player
{
    public interface IMove
    {
        public const float WaitBetweenFire = .2f;
        public const float MinWalkSpeedX = .28f;
        public const float WalkAccelerationX = .14f;
        public const float RunAccelerationX = .21f;
        public const float ReleaseDecelerationX = .25f; // original: .19f;
        public const float SkidDecelerationX = .5f; // .38f;
        public const float SkidTurnaroundSpeedX = 3.5f; // 2.11;
        public const float MaxWalkSpeedX = 5.86f;
        public const float MaxRunSpeedX = 9.61f;
        public float FaceDirectionX { get; set; }
        public float MoveDirectionX { get; set; }
        public float CurrentSpeedX { get; set; }
        public bool IsChangingDirection { get; set; }
        public float AutomaticWalkSpeedX { get; set; }
        public float SpeedXBeforeJump { get; set; }
        public float MidairAccelerationX { get; set; }
        public float MidairDecelerationX { get; set; }
        public float LevelEntryWalkSpeedX { get; }
        public float CastleWalkSpeedX { get; }
        public bool IsClimbingFlagPole { get; set; }
        public Vector2 ClimbFlagPoleVelocity { get; }
        public void Movement();

        public static void OrientSprite(float direction, Transform transform)
        {
            // Assuming default sprites face right
            /******** Switch sprite horizontal orientation 1, -1 */
            switch (direction) {
                case > 0:
                    transform.localScale = Vector2.one;
                    break;
                case < 0:
                    transform.localScale = new Vector2(-1, 1);
                    break;
            }

            // Transform transformCached = transform;
            // transformCached.localScale = FaceDirectionX switch {
            //     > 0 => Vector2.one,
            //     < 0 => new Vector2(-1, 1),
            //     _ => transformCached.localScale
            // };
        }

        public void ResetMovementParams();
        public void ClimbFlagPole();
        public void AutomaticWalk(float walkVelocityX);
    }
}