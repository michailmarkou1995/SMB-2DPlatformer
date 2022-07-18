using UnityEngine;

namespace Core.Player
{
    public partial class PlayerController
    {
        private void SubscribeToEvents()
        {
            PlayerControls.Player.Enable();
            InputFreeze += ResetMovementParams;
        }

        private void DeSubscribeToEvents()
        {
            PlayerControls.Player.Disable();
            InputFreeze -= ResetMovementParams;
        }

        protected override void PerLevelInitialization()
        {
            // Drop Mario at spawn position
            transform.position = FindObjectOfType<global::LevelManager>().FindSpawnPosition();

            // Set correct size
            UpdateSize();

            FireTime1 = 0;
            FireTime2 = 0;
            MStompBox.SetActive(false);
        }

        protected override void AnimationParams()
        {
            MAnimator.SetBool(IsJumpingAnimator, IsJumping);
            MAnimator.SetBool(IsFallingNotFromJumpAnimator, IsFalling && !IsJumping);
            MAnimator.SetBool(IsCrouchingAnimator, IsCrouching);
            MAnimator.SetFloat(AbsSpeedAnimator, Mathf.Abs(CurrentSpeedX));
        }

        /// <summary>
        /// Checks if Player is Grounded and if so, sets the IsGrounded bool to true.
        /// </summary>
        /// <returns>
        /// True if Player is Grounded, false otherwise.
        /// </returns>
        /// <remarks>
        /// Uses either Raycast type BoxCast for all edges check or gameObject "Colliders", it's user's choice.
        /// </remarks>
        private new bool IsGrounded()
        {
            // const float extraHeightText = 1f;
            // Bounds bounds = MBoxCollider2D.bounds;
            // RaycastHit2D raycastHit2D = Physics2D.BoxCast(bounds.center,
            //     bounds.size, 0f, Vector2.down, extraHeightText, GroundLayers);

            // Color rayColor;
            // rayColor = raycastHit2D.collider != null ? Color.green : Color.red;
            // Debug.DrawRay(bounds.center, Vector2.down * (bounds.extents.y + extraHeightText), rayColor);

            // return !(raycastHit2D.collider == null);

            return Physics2D.OverlapPoint(MGroundCheck1.position, GroundLayers) ||
                   Physics2D.OverlapPoint(MGroundCheck2.position, GroundLayers);
        }

        /// <summary>
        /// Movement control for simple Jump and Gravity - SetJumpParams values.
        /// </summary>
        /// <remarks>
        /// <b><i>"IF (base.IsGrounded and JumpButtonHeld)"</i></b><br/>
        /// <b><i>"then"</i></b> Apply these computational values<br/>
        /// <b>JumpSpeedY</b>, <b>JumpUpGravity</b>, <b>JumpDownGravity</b> depending on Players <b>CurrentSpeedX</b>.
        /// </remarks>
        private void SetJumpParams()
        {
            switch (CurrentSpeedX) {
                case < 3.75f:
                    JumpSpeedY = 15f;
                    JumpUpGravity = .47f;
                    JumpDownGravity = 1.64f;
                    break;
                case < 8.67f:
                    JumpSpeedY = 15f;
                    JumpUpGravity = .44f;
                    JumpDownGravity = 1.41f;
                    break;
                default:
                    JumpSpeedY = 18.75f;
                    JumpUpGravity = .59f;
                    JumpDownGravity = 2.11f;
                    break;
            }
        }

        /// <summary>
        /// Movement control for <b><i>"IF (isGrounded==false)"</i></b> and Changed Direction while On Air - SetMidairParams values.
        /// </summary>
        /// <remarks>
        /// <b><i>"IF (!base.IsGrounded)"</i></b><br/>
        /// <b><i>"then"</i></b> apply the below computational values for <b>MidairAccelerationX</b>
        /// to feed to the rest methods below in the flow as "input parameters signature".<br/>
        /// Used with <b>IsChangingDirection</b>, <b>CurrentSpeedX</b>, <b>FaceDirectionX</b>,
        /// taking consideration of <b>SpeedXBeforeJump</b> on "IF" checks.<br/>
        /// Used after "this.method" call to determine <b>IncreaseWithinBound(),</b>
        /// <b>DecreaseWithinBound()</b> methods as parameter input signature.<br/>
        /// </remarks>
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
        
        [SerializeField] private bool _isDying;
        private float _deadUpTimer = .25f;

//	Vector2 deadUpVelocity = new Vector2 (0, 10f);
//	Vector2 deadDownVelocity = new Vector2 (0, -15f);
        public void FreezeAndDie()
        {
            FreezeUserInput();
            _isDying = true;
            MRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            MAnimator.SetTrigger(RespawnAnimator);
            gameObject.layer = LayerMask.NameToLayer("Falling to Kill Plane");
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground Effect";
        }

        /// <summary>
        /// Mario starts climbing flag pole and Disabling User Input and take control of player by Kinematic rigidbody.
        /// </summary>
        public void ClimbFlagPole()
        {
            FreezeUserInput();
            IsClimbingFlagPole = true;
            MAnimator.SetBool(PoleAnimator, true);
            MRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }


        /// <summary>
        /// Player Gets off pole and start walking right e.g., Mario jumps off pole and walks to castle
        /// </summary>
        /// <remarks>
        /// Used after climb flag pole and Triggered by Players OnCollisionEnter2D event.
        /// (other.gameObject.CompareTag("Goal") && IsClimbingFlagPole && bottomHit)
        /// </remarks>
        private void JumpOffPole()
        {
            Transform transformCached = transform;
            Vector3 position = transformCached.position;
            position = new Vector2(position.x + .5f, position.y);
            transformCached.position = position;
            MAnimator.SetBool(PoleAnimator, false);
            AutomaticWalk(CastleWalkSpeedX);
            MRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }

        /// <summary>
        /// Used with AutomaticWalk() to determine if Mario is at the end of the automatic walk sequence.
        /// Automatic movement (e.g. walk to castle sequence) (e.g. walk to castle sequence)
        /// Not in Player Pause Menu.
        /// </summary>
        public void UnfreezeUserInput()
        {
            PlayerControls.Player.Enable();
            InputFreezed = false;
        }

        /// <summary>
        /// Used in AutomaticWalk() to determine if Mario is at the end of the automatic walk sequence.
        /// Used with Toad, Bridge, and FlagPole, PipeWarp to determine if Mario is at the end of the automatic walk sequence.
        /// Not in Player Pause Menu.
        /// </summary>
        public void FreezeUserInput()
        {
            PlayerControls.Player.Disable();
            InputFreezed = true;
            OnInputFreeze();
            JumpButtonHeld = false;

            FaceDirectionX = 0;
            MoveDirectionX = 0;

            CurrentSpeedX = 0;
            SpeedXBeforeJump = 0;
            AutomaticWalkSpeedX = 0;
            AutomaticGravity = NormalGravity;

            IsDashing = false;
            WasDashingBeforeJump = false;
            IsCrouching = false;
            IsChangingDirection = false;
            IsShooting = false;
            // IsJumping = false;
            // IsFalling = false;
            // IsChangedDirOnAirYes = false;

            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // stop all momentum
        }


        public void AutomaticWalk(float walkVelocityX)
        {
            FreezeUserInput();
            if (walkVelocityX != 0) {
                FaceDirectionX = walkVelocityX / Mathf.Abs(walkVelocityX);
            }

            AutomaticWalkSpeedX = Mathf.Abs(walkVelocityX);
        }


        public void AutomaticCrouch()
        {
            FreezeUserInput();
            IsCrouching = true;
        }


        /// <summary>
        /// Mario Size: 1.0f = normal, 0.5f = small, 2.0f = big
        /// </summary>
        public override void UpdateSize()
        {
            GetComponent<Animator>().SetInteger(PlayerSizeAnimator, FindObjectOfType<global::LevelManager>().marioSize);
        }

        /// <summary>
        /// IncreaseWithinBound speed momentum.
        /// </summary>
        /// <param name="val">The current speed.</param>
        /// <param name="delta">The Acceleration/walk speed.</param>
        /// <param name="maxVal">The max speed.</param>
        /// <returns>
        /// Returns the new speed.
        /// </returns>
        /// <remarks>
        /// Used with FixedUpdate on Player Pawn Object
        /// </remarks>
        private static float IncreaseWithinBound(float val, float delta, float maxVal = Mathf.Infinity)
        {
            val += delta;
            if (val > maxVal) {
                val = maxVal;
            }

            return val;
        }

        /// <summary>
        /// DecreaseWithinBound speed momentum.
        /// </summary>
        /// <param name="val">The current speed.</param>
        /// <param name="delta">The "de"-Acceleration/walk speed.</param>
        /// <param name="minVal">The min speed.</param>
        /// <returns>
        /// Returns the new speed.
        /// </returns>
        /// <remarks>
        /// Used with FixedUpdate on Player Pawn Object
        /// </remarks>
        private static float DecreaseWithinBound(float val, float delta, float minVal = 0)
        {
            val -= delta;
            if (val < minVal) {
                val = minVal;
            }

            return val;
        }
    }
}