using System.Collections;
using Abilities.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player {
    public partial class PlayerController
    {
        public bool isAllowedToChangeDirectionOnAir;
        protected override void PlayerControlsSubscribe() {
            PlayerControls = new PlayerInputActions();
            PlayerControls.Player.Move.performed += ctx => FaceDirectionX = ctx.ReadValue<float>();
            PlayerControls.Player.Move.canceled += ctx => FaceDirectionX = 0;

            PlayerControls.Player.Crouch.performed += Crouch_performed;
            PlayerControls.Player.Crouch.canceled += Crouch_canceled;

            PlayerControls.Player.Jump.performed += Jump_performed;
            PlayerControls.Player.Jump.canceled += Jump_canceled;

            PlayerControls.Player.Dash.performed += ctx => IsDashing = true;
            PlayerControls.Player.Dash.canceled += ctx => IsDashing = false;

            PlayerControls.Player.Fire.started += ctx => IsShooting = true;
            PlayerControls.Player.Fire.performed += Shooting;
            PlayerControls.Player.Fire.canceled += ctx => IsShooting = false;
        }

        private void Shooting(InputAction.CallbackContext obj) {
            if (!IsShooting || LevelManager.GetGameStateData.PlayerSize != 2) return;
            FireTime2 = Time.time;

            if (!(FireTime2 - FireTime1 >= WaitBetweenFire)) return;
            MAnimator.SetTrigger(IsFiringAnimator);
            GameObject fireball = Instantiate(this.fireball, firePos.position, Quaternion.identity);
            fireball.GetComponent<MarioFireball>().directionX = transform.localScale.x;
            LevelManager.GetSoundManager.SoundSource.PlayOneShot(LevelManager.GetSoundManager.FireballSound);
            FireTime1 = Time.time;
        }

        /// <summary>
        /// Reset params for automatic movement
        /// </summary>
        private void ResetMovementParams() {
            if (!InputFreezed) return;
            CurrentSpeedX = AutomaticWalkSpeedX;
            MRigidbody2D.gravityScale = AutomaticGravity;
        }

        /// <summary>
        /// Freeze horizontal movement while crouching
        /// </summary>
        /// <param name="context">Callback Data</param>
        private void Crouch_performed(InputAction.CallbackContext context) {
            if (InputFreezed) return;
            IsCrouching = true;
            PlayerControls.Player.Move.Disable();
            CurrentSpeedX = 0;
        }

        private void Crouch_canceled(InputAction.CallbackContext context) {
            if (InputFreezed) return;
            IsCrouching = false;
            PlayerControls.Player.Move.Enable();
        }

        private void Jump_performed(InputAction.CallbackContext context) {
            if (InputFreezed) return;
            MStompBox.SetActive(true);
            IsJumping = true;
            JumpButtonHeld = true;

            /******** Vertical movement */
            if (base.IsGrounded) {
                MRigidbody2D.gravityScale = NormalGravity;
            }

            if (base.IsGrounded && JumpButtonHeld) {
                SetJumpParams();
                MRigidbody2D.velocity = new Vector2(MRigidbody2D.velocity.x, JumpSpeedY);
                IsJumping = true;
                SpeedXBeforeJump = CurrentSpeedX;
                WasDashingBeforeJump = IsDashing;
                LevelManager.GetSoundManager.SoundSource.PlayOneShot(LevelManager.GetGameStateData.PlayerSize == 0
                    ? LevelManager.GetSoundManager.JumpSmallSound
                    : LevelManager.GetSoundManager.JumpSuperSound);
            }

            // else if reverse of !isJumping and not released then it is holding it ... long jump!
            if (MRigidbody2D.velocity.y > 0)
                MRigidbody2D.gravityScale = NormalGravity * JumpUpGravity;
            // Make animation controller swap to isGrounded instead of isJumping if button held when touched ground
            StartCoroutine(JumpButtonHeldDelay());
        }

        private void Jump_canceled(InputAction.CallbackContext context) {
            // if released then it is not holding it ... short jump! .. default gravity immediately
            if (InputFreezed) return;
            JumpButtonHeld = false;
            MRigidbody2D.gravityScale = NormalGravity * JumpDownGravity;
            // Make animator swap to isGrounded and allow physics to process isJumping false conditions(when exit secret pipe)
            StartCoroutine(GroundDelay());
        }

        private IEnumerator GroundDelay() {
            // Make animation controller swap to isGrounded instead of isJumping if button held when touched ground
            yield return new WaitUntil(() => base.IsGrounded);
            IsJumping = false;
            MStompBox.SetActive(false);
        }

        private IEnumerator JumpButtonHeldDelay() {
            yield return new WaitForSeconds(.1f);
            StartCoroutine(GroundDelay());
        }

        private void Movement() {
            /******** Horizontal movement on ground */
            if (base.IsGrounded) {
                // Change Sprite direction and movement if player moves in opposite direction on air
                if (IsChangedDirOnAirYes) {
                    IsChangedDirOnAirYes = false;
                    FaceDirectionX = -FaceDirectionX;
                }

                // If holding directional button, accelerate until reach max walk speed
                // If holding Dash, accelerate until reach max run speed
                if (FaceDirectionX != 0) {
                    switch (CurrentSpeedX) {
                        case 0:
                            CurrentSpeedX = MinWalkSpeedX;
                            break;
                        case < MaxWalkSpeedX:
                            CurrentSpeedX = IncreaseWithinBound(CurrentSpeedX, WalkAccelerationX, MaxWalkSpeedX);
                            break;
                        default:
                        {
                            if (IsDashing && CurrentSpeedX < MaxRunSpeedX) {
                                CurrentSpeedX = IncreaseWithinBound(CurrentSpeedX, RunAccelerationX, MaxRunSpeedX);
                            }

                            break;
                        }
                    }
                }

                // Decelerate upon release of directional button
                else if (CurrentSpeedX > 0) {
                    CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, ReleaseDecelerationX, 0);
                }

                // If change direction, skid until lose all momentum then turn around
                if (IsChangingDirection) {
                    if (CurrentSpeedX > SkidTurnaroundSpeedX) {
                        MoveDirectionX = -FaceDirectionX;
                        MAnimator.SetBool(IsSkiddingAnimator, true);
                        CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, SkidDecelerationX, 0);
                    } else {
                        MoveDirectionX = FaceDirectionX;
                        MAnimator.SetBool(IsSkiddingAnimator, false);
                    }
                } else {
                    MAnimator.SetBool(IsSkiddingAnimator, false);
                }

                /******** Horizontal movement on air */
            } else {
                SetMidairParams();

                // Holding Dash while in midair has no effect
                if (FaceDirectionX != 0) {
                    switch (CurrentSpeedX) {
                        case 0:
                            CurrentSpeedX = MinWalkSpeedX;
                            break;
                        case < MaxWalkSpeedX:
                            CurrentSpeedX = IncreaseWithinBound(CurrentSpeedX, MidairAccelerationX, MaxWalkSpeedX);
                            break;
                        default:
                        {
                            if (WasDashingBeforeJump && CurrentSpeedX < MaxRunSpeedX) {
                                CurrentSpeedX = IncreaseWithinBound(CurrentSpeedX, MidairAccelerationX, MaxRunSpeedX);
                            }

                            break;
                        }
                    }
                } else if (CurrentSpeedX > 0) {
                    CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, ReleaseDecelerationX, 0);
                } else if (!PlayerControls.Player.Jump.triggered && !IsJumping && !IsFalling) {
                    MRigidbody2D.gravityScale = NormalGravity * 1.0f; //JumpDownGravity; //1.5f;
                }

                // If change direction, decelerate but keep facing move direction
                // and change sprite direction on upon touching ground
                if (!isAllowedToChangeDirectionOnAir) {
                    if (IsChangingDirection) {
                        IsChangedDirOnAirYes = true;
                        FaceDirectionX = MoveDirectionX;
                        CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, MidairDecelerationX, 0);
                    }
                } else {
                    IsChangedDirOnAirYes = false;
                    FaceDirectionX = -MoveDirectionX;
                    CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, MidairDecelerationX, 0);
                }

            }

            // Disable Stomp Box if not falling down
            MStompBox.SetActive(IsFalling);

            /******** Switch sprite horizontal orientation 1, -1 */
            Transform transformCached = transform;
            transformCached.localScale = FaceDirectionX switch {
                > 0 => Vector2.one,
                < 0 => new Vector2(-1, 1),
                _ => transformCached.localScale
            };

            /******** Horizontal movement force*/
            MRigidbody2D.velocity = new Vector2(MoveDirectionX * CurrentSpeedX, MRigidbody2D.velocity.y);

            if (FaceDirectionX != 0 && !IsChangingDirection) {
                MoveDirectionX = FaceDirectionX;
            }
        }
    }
}