using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class Move : MonoBehaviour, IMove
    {
        // [SerializeField] private float castleWalkSpeedX = 5.86f;
        // [SerializeField] private float levelEntryWalkSpeedX = 3.05f;
        public float FaceDirectionX { get; set; }
        public float MoveDirectionX { get; set; }
        public bool IsChangingDirection { get; set; }
        public float CurrentSpeedX { get; set; }
        public float AutomaticWalkSpeedX { get; set; }
        public float SpeedXBeforeJump { get; set; }
        public float MidairAccelerationX { get; set; }
        public float MidairDecelerationX { get; set; }
        [field: SerializeField] public float LevelEntryWalkSpeedX { get; set; } = 5.86f;
        [field: SerializeField] public float CastleWalkSpeedX { get; set; } = 3.05f;
        public bool IsClimbingFlagPole { get; set; }
        public Vector2 ClimbFlagPoleVelocity { get; } = new(0, -10f);


        private IPlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<IPlayerController>();
        }

        public void Movement()
        {
            /******** Horizontal movement on ground */
            if (_playerController.GetGroundCheck.IsGrounded) {
                // Change Sprite direction and movement if player moves in opposite direction on air
                if (_playerController.GetJump.IsChangedDirOnAirYes) {
                    _playerController.GetJump.IsChangedDirOnAirYes = false;
                    FaceDirectionX = -FaceDirectionX;
                }

                // If holding directional button, accelerate until reach max walk speed
                // If holding Dash, accelerate until reach max run speed
                if (FaceDirectionX != 0) {
                    switch (CurrentSpeedX) {
                        case 0:
                            CurrentSpeedX = IMove.MinWalkSpeedX;
                            break;
                        case < IMove.MaxWalkSpeedX:
                            CurrentSpeedX =
                                IncreaseWithinBound(CurrentSpeedX, IMove.WalkAccelerationX, IMove.MaxWalkSpeedX);
                            break;
                        default:
                        {
                            if (_playerController.GetDash.IsDashing && CurrentSpeedX < IMove.MaxRunSpeedX) {
                                CurrentSpeedX =
                                    IncreaseWithinBound(CurrentSpeedX, IMove.RunAccelerationX, IMove.MaxRunSpeedX);
                            }

                            break;
                        }
                    }
                }

                // Decelerate upon release of directional button
                else if (CurrentSpeedX > 0) {
                    CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, IMove.ReleaseDecelerationX, 0);
                }

                // If change direction, skid until lose all momentum then turn around
                if (IsChangingDirection) {
                    if (CurrentSpeedX > IMove.SkidTurnaroundSpeedX) {
                        MoveDirectionX = -FaceDirectionX;
                        PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(PlayerAnimatorStatic.IsSkiddingAnimator,
                            true);
                        CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, IMove.SkidDecelerationX, 0);
                    } else {
                        MoveDirectionX = FaceDirectionX;
                        PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(PlayerAnimatorStatic.IsSkiddingAnimator,
                            false);
                    }
                } else {
                    PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(PlayerAnimatorStatic.IsSkiddingAnimator,
                        false);
                }

                /******** Horizontal movement on air */
            } else {
                SetMidairParams();

                // Holding Dash while in midair has no effect
                if (FaceDirectionX != 0) {
                    switch (CurrentSpeedX) {
                        case 0:
                            CurrentSpeedX = IMove.MinWalkSpeedX;
                            break;
                        case < IMove.MaxWalkSpeedX:
                            CurrentSpeedX =
                                IncreaseWithinBound(CurrentSpeedX, MidairAccelerationX, IMove.MaxWalkSpeedX);
                            break;
                        default:
                        {
                            if (_playerController.GetDash.WasDashingBeforeJump && CurrentSpeedX < IMove.MaxRunSpeedX) {
                                CurrentSpeedX =
                                    IncreaseWithinBound(CurrentSpeedX, MidairAccelerationX, IMove.MaxRunSpeedX);
                            }

                            break;
                        }
                    }
                } else if (CurrentSpeedX > 0) {
                    CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, IMove.ReleaseDecelerationX, 0);
                } else if (!_playerController.PlayerControls.Player.Jump.triggered
                           && !_playerController.GetJump.IsJumping
                           && !_playerController.GetJump.IsFalling) {
                    _playerController.MRigidbody2D.gravityScale =
                        _playerController.GetJump.NormalGravity * 1.0f; //JumpDownGravity; //1.5f;
                }

                // If change direction, decelerate but keep facing move direction
                // and change sprite direction on upon touching ground
                if (IsChangingDirection) {
                    _playerController.GetJump.IsChangedDirOnAirYes = true;
                    FaceDirectionX = MoveDirectionX;
                    CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, MidairDecelerationX, 0);
                }
            }

            // Disable Stomp Box if not falling down
            _playerController.MStompBox.SetActive(_playerController.GetJump.IsFalling);

            /******** Switch sprite horizontal orientation 1, -1 */
            Transform transformCached = transform;
            transformCached.localScale = FaceDirectionX switch {
                > 0 => Vector2.one,
                < 0 => new Vector2(-1, 1),
                _ => transformCached.localScale
            };

            /******** Horizontal movement force*/
            _playerController.MRigidbody2D.velocity =
                new Vector2(MoveDirectionX * CurrentSpeedX, _playerController.MRigidbody2D.velocity.y);

            if (FaceDirectionX != 0 && !IsChangingDirection) {
                MoveDirectionX = FaceDirectionX;
            }
        }

        private static float IncreaseWithinBound(float val, float delta, float maxVal = Mathf.Infinity)
        {
            val += delta;
            if (val > maxVal) {
                val = maxVal;
            }

            return val;
        }

        private static float DecreaseWithinBound(float val, float delta, float minVal = 0)
        {
            val -= delta;
            if (val < minVal) {
                val = minVal;
            }

            return val;
        }

        private void SetMidairParams()
        {
            if (CurrentSpeedX < 5.86f) {
                MidairAccelerationX = .14f;
                MidairDecelerationX = SpeedXBeforeJump < 6.80f ? .14f : .19f;
            } else {
                MidairAccelerationX = .21f;
                MidairDecelerationX = .21f;
            }
        }

        public void ResetMovementParams()
        {
            if (!_playerController.GetMovementFreeze.InputFreezed) return;
            CurrentSpeedX = AutomaticWalkSpeedX;
            _playerController.MRigidbody2D.gravityScale = _playerController.GetJump.AutomaticGravity;
        }

        public void ClimbFlagPole()
        {
            _playerController.GetMovementFreeze.FreezeUserInput();
            IsClimbingFlagPole = true;
            PlayerAnimatorStatic.PlayerAnimatorComponent.SetBool(PlayerAnimatorStatic.PoleAnimator, true);
            _playerController.MRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        public void AutomaticWalk(float walkVelocityX)
        {
            _playerController.GetMovementFreeze.FreezeUserInput();
            if (walkVelocityX != 0) {
                FaceDirectionX = walkVelocityX / Mathf.Abs(walkVelocityX);
            }

            AutomaticWalkSpeedX = Mathf.Abs(walkVelocityX);
        }
    }
}