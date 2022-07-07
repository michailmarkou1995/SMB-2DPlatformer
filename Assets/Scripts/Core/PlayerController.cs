using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

//TODO add singleton?
//TODO add interfaces
//TODO Unit Tests your behavior
//TODO add documentation
//TODO add comments

namespace Core {
    public class PlayerController : MonoBehaviour {
        // Inspector Components
        private Transform mGroundCheck1, mGroundCheck2;
        private GameObject mStompBox;
        private Rigidbody2D mRigidbody2D; // body is used for applying force to the player
        private CircleCollider2D mCircleCollider2D;

        // Player Controls
        private PlayerInputActions playerControls;

        // Animator state machine with animation clips attached to it
        private Animator mAnimatorPlayer;

        // Cached Animator Parameters
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private static readonly int AbsSpeed = Animator.StringToHash("absSpeed");

        // Layers property of inspector menu
        [FormerlySerializedAs("GroundLayers")] public LayerMask groundLayers;

        private float movement;
        private float faceDirectionX;
        private float moveDirectionX;
        private float normalGravity;
        private float currentSpeedX;
        private float speedXBeforeJump;
        private const float MinWalkSpeedX = .28f;
        private const float WalkAccelerationX = .14f;
        private const float ReleaseDecelerationX = .25f; // original: .19f;
        private const float SkidDecelerationX = .5f; // .38f;
        private const float SkidTurnaroundSpeedX = 3.5f; // 2.11;
        private const float MaxWalkSpeedX = 5.86f;
        private float jumpSpeedY;
        private float jumpUpGravity;
        private float jumpDownGravity;
        private float midairAccelerationX;
        private float midairDecelerationX;
        private float automaticWalkSpeedX;
        private float automaticGravity;

        private bool isMoving;
        private bool isGrounded;
        private bool isJumping;
        private bool isChangingDirection;
        private bool isShooting;
        public bool isCrouching;
        private bool isJumpButtonHeld;
        private bool isChangedDirOnAirYes;

        // Start is called before the first frame update
        private void Awake() {
            playerControls = new PlayerInputActions();
            playerControls.Player.Move.performed += ctx => faceDirectionX = ctx.ReadValue<float>();
            playerControls.Player.Move.canceled += ctx => faceDirectionX = 0;
            playerControls.Player.Crouch.performed += Crouch_performed;
            playerControls.Player.Crouch.canceled += Crouch_canceled;

            playerControls.Player.Jump.performed += Jump_performed;
            playerControls.Player.Jump.canceled += Jump_canceled;
        }

        // Freeze horizontal movement while crouching
        private void Crouch_performed(InputAction.CallbackContext context) {
            isCrouching = true;
            playerControls.Player.Move.Disable();
            currentSpeedX = 0;
        }

        private void Crouch_canceled(InputAction.CallbackContext context) {
            isCrouching = false;
            playerControls.Player.Move.Enable();
        }

        private void Jump_performed(InputAction.CallbackContext context) {
            isJumpButtonHeld = true;

            /******** Vertical movement */
            if (isGrounded) {
                isJumping = false;
                mRigidbody2D.gravityScale = normalGravity;
            }

            if (!isJumping) {
                if (isGrounded && isJumpButtonHeld) {
                    SetJumpParams();
                    mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, jumpSpeedY);
                    isJumping = true;
                    speedXBeforeJump = currentSpeedX;
                }
            }

            // else if reverse of !isJumping and not released then it is holding it ... long jump!
            if (mRigidbody2D.velocity.y > 0)
                mRigidbody2D.gravityScale = normalGravity * jumpUpGravity;
        }

        private void Jump_canceled(InputAction.CallbackContext context) {
            // if released then it is not holding it ... short jump! .. default gravity immediately
            isJumpButtonHeld = false;
            mRigidbody2D.gravityScale = normalGravity * jumpDownGravity;
        }

        private void OnEnable() {
            playerControls.Player.Enable();
        }

        private void OnDisable() {
            playerControls.Player.Disable();
        }

        private void Start() {
            mGroundCheck1 = transform.Find("Ground Check 1");
            mGroundCheck2 = transform.Find("Ground Check 2");
            mRigidbody2D = GetComponent<Rigidbody2D>();
            mCircleCollider2D = GetComponent<CircleCollider2D>();
            mAnimatorPlayer = GetComponent<Animator>();
            normalGravity = mRigidbody2D.gravityScale;

            // Drop Mario at spawn position
            transform.position = FindObjectOfType<LevelManager>().FindSpawnPosition();
        }

        // Update is called once per frame
        private void Update() {
            isGrounded = Physics2D.OverlapPoint(mGroundCheck1.position, groundLayers) ||
                         Physics2D.OverlapPoint(mGroundCheck2.position, groundLayers);
            isChangingDirection = currentSpeedX > 0 && faceDirectionX * moveDirectionX < 0;
        }

        private void FixedUpdate() {
            /******** Horizontal movement on ground */
            if (isGrounded) {
                if (isChangedDirOnAirYes) {
                    isChangedDirOnAirYes = false;
                    faceDirectionX = -faceDirectionX;
                }

                // If holding directional button, accelerate until reach max walk speed
                if (faceDirectionX != 0) {
                    switch (currentSpeedX) {
                        case 0:
                            currentSpeedX = MinWalkSpeedX;
                            break;
                        case < MaxWalkSpeedX:
                            currentSpeedX = IncreaseWithinBound(currentSpeedX, WalkAccelerationX, MaxWalkSpeedX);
                            break;
                    }
                }
                // Decelerate upon release of directional button
                else if (currentSpeedX > 0) {
                    currentSpeedX = DecreaseWithinBound(currentSpeedX, ReleaseDecelerationX);
                }

                // If change direction, skid until lose all momentum then turn around
                if (isChangingDirection) {
                    if (currentSpeedX > SkidTurnaroundSpeedX) {
                        moveDirectionX = -faceDirectionX;
                        currentSpeedX = DecreaseWithinBound(currentSpeedX, SkidDecelerationX);
                    } else {
                        moveDirectionX = faceDirectionX;
                    }
                }

                // Freeze horizontal movement while crouching
                if (isCrouching) {
                    currentSpeedX = 0;
                }
                /******** Horizontal movement on air */
            } else {
                SetMidairParams();

                if (faceDirectionX != 0) {
                    if (currentSpeedX == 0) {
                        currentSpeedX = MinWalkSpeedX;
                    } else if (currentSpeedX < MaxWalkSpeedX) {
                        currentSpeedX = IncreaseWithinBound(currentSpeedX, midairAccelerationX, MaxWalkSpeedX);
                    }
                } else if (currentSpeedX > 0) {
                    currentSpeedX = DecreaseWithinBound(currentSpeedX, ReleaseDecelerationX);
                }

                // If change direction, decelerate but keep facing move direction
                if (isChangingDirection) {
                    isChangedDirOnAirYes = true;
                    faceDirectionX = moveDirectionX; // -moveDirectionX; make to change on fly direction and run to it
                    currentSpeedX = DecreaseWithinBound(currentSpeedX, midairDecelerationX);
                }
            }

            // /******** Horizontal orientation */
            Transform transformCached = transform;
            transformCached.localScale = faceDirectionX switch {
                /******** Horizontal orientation */
                > 0 => Vector2.one, // facing right
                < 0 => new Vector2(-1, 1),
                _ => transformCached.localScale
            };

            /******** Set params */
            mRigidbody2D.velocity = new Vector2(moveDirectionX * currentSpeedX, mRigidbody2D.velocity.y);

            mAnimatorPlayer.SetBool(IsJumping, isJumping);
            mAnimatorPlayer.SetFloat(AbsSpeed, Mathf.Abs(currentSpeedX));

            if (faceDirectionX != 0 && !isChangingDirection) {
                moveDirectionX = faceDirectionX; // -faceDirectionX; funny spin on run and jump
            }
        }

        /****************** Movement control */
        private void SetJumpParams() {
            switch (currentSpeedX) {
                case < 3.75f:
                    jumpSpeedY = 15f;
                    jumpUpGravity = .47f;
                    jumpDownGravity = 1.64f;
                    break;
                case < 8.67f:
                    jumpSpeedY = 15f;
                    jumpUpGravity = .44f;
                    jumpDownGravity = 1.41f;
                    break;
                default:
                    jumpSpeedY = 18.75f;
                    jumpUpGravity = .59f;
                    jumpDownGravity = 2.11f;
                    break;
            }
        }

        private void SetMidairParams() {
            if (currentSpeedX < 5.86f) {
                midairAccelerationX = .14f;
                midairDecelerationX = speedXBeforeJump < 6.80f ? .14f : .19f;
            } else {
                midairAccelerationX = .21f;
                midairDecelerationX = .21f;
            }
        }

        private static float IncreaseWithinBound(float val, float delta, float maxVal = Mathf.Infinity) {
            val += delta;
            if (val > maxVal) {
                val = maxVal;
            }

            return val;
        }

        private static float DecreaseWithinBound(float val, float delta, float minVal = 0) {
            val -= delta;
            if (val < minVal) {
                val = minVal;
            }

            return val;
        }

        // Checks to see if the player is colliding with something and deems the player as "grounded", allowing them to jump.
        private void OnCollisionStay2D(Collision2D col) {
            isJumping = false;
        }

        // Checks to see if the player is jumping
        private void OnCollisionExit2D(Collision2D col) {
            isJumping = true;
        }
    }
}