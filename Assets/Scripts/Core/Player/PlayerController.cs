using Core.NPC;
using UnityEditor;
using UnityEngine;

namespace Core.Player
{
    /// <summary>
    /// <b>PlayerControllerMario</b> is the main class for the player
    /// which is splitted in three parts and 1 super class/inheritance.<br/>
    /// <list type="bullet">
    /// <item>File name 1 main-file-partial: <b><i>PlayerControllerMario.cs</i></b>, Class name is <b><i>PlayerControllerMario</i></b></item>
    /// <item>File name 2 file-partial: <b><i>PlayerControllerMario_Movement.cs</i></b>, Class name is <b><i>PlayerControllerMario</i></b></item>
    /// <item>File name 3 file-partial: <b><i>PlayerControllerMario_Misc.cs</i></b>, Class name is <b><i>PlayerControllerMario</i></b></item>
    /// </list>
    /// </summary>
    public partial class PlayerController : PlayerBase
    {
        private void Awake()
        {
            PlayerControlsSubscribe();
            InitializationComponents();
        }

        private void Start()
        {
            PerLevelInitialization();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            DeSubscribeToEvents();
        }

        private void Update()
        {
            //Debug.Log(CastleWalkSpeedX);
            base.IsGrounded = IsGrounded();
            IsFalling = MRigidbody2D.velocity.y < 0 && !base.IsGrounded;
            IsChangingDirection = CurrentSpeedX > 0 && FaceDirectionX * MoveDirectionX < 0;

            if (!InputFreezed || LevelManager.GetGameStateData.GamePaused) return;
            if (_isDying) {
                _deadUpTimer -= Time.unscaledDeltaTime;
                if (_deadUpTimer > 0) {
                    // TODO MovePosition not working
//					m_Rigidbody2D.MovePosition (m_Rigidbody2D.position + deadUpVelocity * Time.unscaledDeltaTime);
                    gameObject.transform.position += Vector3.up * .22f;
                } else {
//					m_Rigidbody2D.MovePosition (m_Rigidbody2D.position + deadDownVelocity * Time.unscaledDeltaTime);
                    gameObject.transform.position += Vector3.down * .2f;
                }
            } else if (IsClimbingFlagPole) {
                MRigidbody2D.MovePosition(MRigidbody2D.position + ClimbFlagPoleVelocity * Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            // Player movement (horizontal) and Jumping (vertical)
            Movement();

            /******** Set animation params */
            AnimationParams();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // ignore collisions with stomp box enabled .. stomping enemy should do no damage
            if (MStompBox.activeSelf && MStompBox.GetComponent<Collider2D>().isTrigger) return;
            MCapsuleCollider2D.enabled = other.gameObject.CompareTag("Pipe");
            Vector2 normal = other.contacts[0].normal;
            Vector2 bottomSide = new(0f, 1f);
            bool bottomHit = normal == bottomSide;

            if (other.gameObject.tag.Contains("Enemy")) {
                // TODO: koopa shell static does no damage
                Enemy enemy = other.gameObject.GetComponent<Enemy>();

                if (!LevelManager.GetPlayerAbilities.IsInvincible()) {
                    if (!other.gameObject.GetComponent<KoopaShell>() ||
                        other.gameObject.GetComponent<KoopaShell>()
                            .isRolling || // non-rolling shell should do no damage
                        !bottomHit || (!enemy.isBeingStomped)) {
                        LevelManager.GetPlayerAbilities.MarioPowerDown();
                    }
                } else if (LevelManager.GetPlayerAbilities.IsInvincibleStarman) {
                    LevelManager.GetPlayerAbilities.MarioStarmanTouchEnemy(enemy);
                }
            } else if (other.gameObject.CompareTag("Goal") && IsClimbingFlagPole && bottomHit) {
                IsClimbingFlagPole = false;
                JumpOffPole();
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!collision.gameObject.tag.Contains("Enemy")) return;
            Vector2 normal = collision.contacts[0].normal;
            Vector2 bottomSide = new(0f, 1f);
            bool bottomHit = normal == bottomSide;
            // TODO: koopa shell static does no damage
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (!LevelManager.GetPlayerAbilities.IsInvincible()) {
                if (!collision.gameObject.GetComponent<KoopaShell>() ||
                    collision.gameObject.GetComponent<KoopaShell>()
                        .isRolling || // non-rolling shell should do no damage
                    !bottomHit || (!enemy.isBeingStomped)) {
                    LevelManager.GetPlayerAbilities.MarioPowerDown();
                }
            } else if (LevelManager.GetPlayerAbilities.IsInvincibleStarman) {
                LevelManager.GetPlayerAbilities.MarioStarmanTouchEnemy(enemy);
            }
        }
    }
}

// using System;
// using System.Collections;
// using System.Globalization;
// using _common;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.Serialization;
//
// namespace Core.Player {
//     public partial class PlayerControllerMario : MonoBehaviour, IPlayer {
//         private global::LevelManager LevelManager;
//         private Transform MGroundCheck1, MGroundCheck2;
//         private GameObject MStompBox;
//         private Animator MAnimator;
//         private Rigidbody2D MRigidbody2D;
//
//         // Player Controls
//         private PlayerInputActions PlayerControls;
//
//         private int GroundLayers;
//
//         //[FormerlySerializedAs("GroundLayers")] public LayerMask groundLayers;
//         [FormerlySerializedAs("Fireball")] public GameObject fireball;
//         [FormerlySerializedAs("FirePos")] public Transform firePos;
//
//         private float fireTime1, fireTime2;
//         private float FaceDirectionX;
//         private float MoveDirectionX;
//         private float NormalGravity;
//
//         private float CurrentSpeedX;
//         private float SpeedXBeforeJump;
//
//         private const float WaitBetweenFire = .2f;
//         private const float MinWalkSpeedX = .28f;
//         private const float WalkAccelerationX = .14f;
//         private const float RunAccelerationX = .21f;
//         private const float ReleaseDecelerationX = .25f; // original: .19f;
//         private const float SkidDecelerationX = .5f; // .38f;
//         private const float SkidTurnaroundSpeedX = 3.5f; // 2.11;
//         private const float MaxWalkSpeedX = 5.86f;
//         private const float MaxRunSpeedX = 9.61f;
//
//         private float jumpSpeedY;
//         private float jumpUpGravity;
//         private float jumpDownGravity; //= 2.11f;
//         private float midairAccelerationX;
//         private float midairDecelerationX;
//
//         private float AutomaticWalkSpeedX;
//         private float AutomaticGravity;
//
//         [FormerlySerializedAs("castleWalkSpeedX")]
//         public float CastleWalkSpeedX = 5.86f;
//
//         [FormerlySerializedAs("levelEntryWalkSpeedX")]
//         public float LevelEntryWalkSpeedX = 3.05f;
//
//         private bool IsGrounded;
//         private bool IsDashing;
//         private bool IsFalling;
//         private bool IsJumping;
//         private bool IsChangingDirection;
//         private bool WasDashingBeforeJump;
//         private bool IsShooting;
//         [FormerlySerializedAs("isCrouching")] public bool IsCrouching;
//         private bool IsChangedDirOnAirYes;
//         private bool JumpButtonHeld;
//         public bool InputFreezed;
//
//         public event Action InputFreeze;
//
//         public void testme() {
//             Debug.Log("testme");
//         }
//
//         private void Awake() {
//             PlayerControls = new PlayerInputActions();
//             PlayerControls.Player.Move.performed += ctx =>
//             {
//                 if (!InputFreezed) {
//                     FaceDirectionX = ctx.ReadValue<float>();
//                 }
//             };
//             PlayerControls.Player.Move.canceled += ctx => FaceDirectionX = 0;
//
//             PlayerControls.Player.Crouch.performed += Crouch_performed;
//             PlayerControls.Player.Crouch.canceled += Crouch_canceled;
//
//             PlayerControls.Player.Jump.performed += Jump_performed;
//             PlayerControls.Player.Jump.canceled += Jump_canceled;
//
//             PlayerControls.Player.Dash.performed += ctx => IsDashing = true;
//             PlayerControls.Player.Dash.canceled += ctx => IsDashing = false;
//
//             PlayerControls.Player.Fire.started += ctx => IsShooting = true;
//             PlayerControls.Player.Fire.performed += Shooting;
//             PlayerControls.Player.Fire.canceled += ctx => IsShooting = false;
//
//             LevelManager = FindObjectOfType<global::LevelManager>();
//             MGroundCheck1 = transform.Find("Ground Check 1");
//             MGroundCheck2 = transform.Find("Ground Check 2");
//             MStompBox = transform.Find("Stomp Box").gameObject;
//             MAnimator = GetComponent<Animator>();
//             MRigidbody2D = GetComponent<Rigidbody2D>();
//             NormalGravity = MRigidbody2D.gravityScale;
//
//             GroundLayers = (1 << LayerMask.NameToLayer("Ground")
//                             | 1 << LayerMask.NameToLayer("Block")
//                             | 1 << LayerMask.NameToLayer("Goal")
//                             | 1 << LayerMask.NameToLayer("Player Detector")
//                             | 1 << LayerMask.NameToLayer("Moving Platform"));
//         }
//
//         private void Shooting(InputAction.CallbackContext obj) {
//             /******** Shooting */
//             if (!IsShooting || LevelManager.marioSize != 2) return;
//             fireTime2 = Time.time;
//
//             if (!(fireTime2 - fireTime1 >= WaitBetweenFire)) return;
//             MAnimator.SetTrigger(IsFiringAnimator);
//             GameObject fireball = Instantiate(this.fireball, firePos.position, Quaternion.identity);
//             fireball.GetComponent<MarioFireball>().directionX = transform.localScale.x;
//             LevelManager.soundSource.PlayOneShot(LevelManager.fireballSound);
//             fireTime1 = Time.time;
//         }
//
//         private void ResetMovementParams() {
//             /******** Reset params for automatic movement */
//             if (!InputFreezed) return;
//             CurrentSpeedX = AutomaticWalkSpeedX;
//             MRigidbody2D.gravityScale = AutomaticGravity;
//         }
//
//         // Freeze horizontal movement while crouching
//         private void Crouch_performed(InputAction.CallbackContext context) {
//             // Freeze horizontal and vertical movement while crouching
//             if (InputFreezed) return;
//             IsCrouching = true;
//             PlayerControls.Player.Move.Disable();
//             CurrentSpeedX = 0;
//         }
//
//         private void Crouch_canceled(InputAction.CallbackContext context) {
//             if (InputFreezed) return;
//             IsCrouching = false;
//             PlayerControls.Player.Move.Enable();
//         }
//
//         private void Jump_performed(InputAction.CallbackContext context) {
//             if (InputFreezed) return;
//             MStompBox.SetActive(true);
//             IsJumping = true;
//             JumpButtonHeld = true;
//
//             /******** Vertical movement */
//             if (IsGrounded) {
//                 MRigidbody2D.gravityScale = NormalGravity;
//             }
//
//             if (IsGrounded && JumpButtonHeld) {
//                 SetJumpParams();
//                 MRigidbody2D.velocity = new Vector2(MRigidbody2D.velocity.x, jumpSpeedY);
//                 IsJumping = true;
//                 SpeedXBeforeJump = CurrentSpeedX;
//                 WasDashingBeforeJump = IsDashing;
//                 LevelManager.soundSource.PlayOneShot(LevelManager.marioSize == 0
//                     ? LevelManager.jumpSmallSound
//                     : LevelManager.jumpSuperSound);
//             }
//
//             // else if reverse of !isJumping and not released then it is holding it ... long jump!
//             if (MRigidbody2D.velocity.y > 0)
//                 MRigidbody2D.gravityScale = NormalGravity * jumpUpGravity;
//             // Make animation controller swap to isGrounded instead of isJumping if button held when touched ground
//             StartCoroutine(JumpButtonHeldDelay());
//         }
//
//         private void Jump_canceled(InputAction.CallbackContext context) {
//             // if released then it is not holding it ... short jump! .. default gravity immediately
//             if (InputFreezed) return;
//             JumpButtonHeld = false;
//             MRigidbody2D.gravityScale = NormalGravity * jumpDownGravity;
//             // Make animator swap to isGrounded and allow physics to process isJumping false conditions(when exit secret pipe)
//             StartCoroutine(GroundDelay());
//         }
//
//         private IEnumerator GroundDelay() {
//             // Make animation controller swap to isGrounded instead of isJumping if button held when touched ground
//             yield return new WaitUntil(() => IsGrounded);
//             IsJumping = false;
//             MStompBox.SetActive(false);
//         }
//
//         private IEnumerator JumpButtonHeldDelay() {
//             yield return new WaitForSeconds(.1f);
//             StartCoroutine(GroundDelay());
//         }
//
//         private void OnEnable() {
//             PlayerControls.Player.Enable();
//             InputFreeze += ResetMovementParams;
//         }
//
//         private void OnDisable() {
//             PlayerControls.Player.Disable();
//             InputFreeze -= ResetMovementParams;
//         }
//
//
//         // Use this for initialization
//         private void Start() {
//             // Drop Mario at spawn position
//             transform.position = FindObjectOfType<global::LevelManager>().FindSpawnPosition();
//
//             // Set correct size
//             UpdateSize();
//
//             fireTime1 = 0;
//             fireTime2 = 0;
//             MStompBox.SetActive(false);
//         }
//
//
//         /****************** Automatic movement sequences */
//         private void Update() {
//             Debug.Log("JumpDownGravity: " + jumpDownGravity
//                                         + " automatic gravity scale: "
//                                         + MRigidbody2D.gravityScale
//                                         + " normalGravity "
//                                         + NormalGravity);
//             if (Input.GetKey(KeyCode.G)) {
//                 FreezeUserInput();
//             }
//
//             if (Input.GetKey(KeyCode.H)) {
//                 UnfreezeUserInput();
//             }
//
//             IsFalling = MRigidbody2D.velocity.y < 0 && !IsGrounded;
//             IsGrounded = Physics2D.OverlapPoint(MGroundCheck1.position, GroundLayers) ||
//                          Physics2D.OverlapPoint(MGroundCheck2.position, GroundLayers);
//             IsChangingDirection = CurrentSpeedX > 0 && FaceDirectionX * MoveDirectionX < 0;
//             
//             MAnimator.SetBool(IsJumpingAnimator, IsJumping);
//             MAnimator.SetBool(IsFallingNotFromJumpAnimator, IsFalling && !IsJumping);
//             MAnimator.SetBool(IsCrouchingAnimator, IsCrouching);
//             MAnimator.SetFloat(AbsSpeedAnimator, Mathf.Abs(CurrentSpeedX));
//
//             if (!InputFreezed || LevelManager.gamePaused) return;
//             if (isDying) {
//                 deadUpTimer -= Time.unscaledDeltaTime;
//                 if (deadUpTimer > 0) {
//                     // TODO MovePosition not working
// //					m_Rigidbody2D.MovePosition (m_Rigidbody2D.position + deadUpVelocity * Time.unscaledDeltaTime);
//                     gameObject.transform.position += Vector3.up * .22f;
//                 } else {
// //					m_Rigidbody2D.MovePosition (m_Rigidbody2D.position + deadDownVelocity * Time.unscaledDeltaTime);
//                     gameObject.transform.position += Vector3.down * .2f;
//                 }
//             } else if (isClimbingFlagPole) {
//                 MRigidbody2D.MovePosition(MRigidbody2D.position + climbFlagPoleVelocity * Time.deltaTime);
//             }
//         }
//
//         private void FixedUpdate() {
//             /******** Horizontal movement on ground */
//             if (IsGrounded) {
//                 if (IsChangedDirOnAirYes) {
//                     IsChangedDirOnAirYes = false;
//                     FaceDirectionX = -FaceDirectionX;
//                 }
//
//                 // If holding directional button, accelerate until reach max walk speed
//                 // If holding Dash, accelerate until reach max run speed
//                 if (FaceDirectionX != 0) {
//                     switch (CurrentSpeedX) {
//                         case 0:
//                             CurrentSpeedX = MinWalkSpeedX;
//                             break;
//                         case < MaxWalkSpeedX:
//                             CurrentSpeedX = IncreaseWithinBound(CurrentSpeedX, WalkAccelerationX, MaxWalkSpeedX);
//                             break;
//                         default:
//                         {
//                             if (IsDashing && CurrentSpeedX < MaxRunSpeedX) {
//                                 CurrentSpeedX = IncreaseWithinBound(CurrentSpeedX, RunAccelerationX, MaxRunSpeedX);
//                             }
//
//                             break;
//                         }
//                     }
//                 }
//
//                 // Decelerate upon release of directional button
//                 else if (CurrentSpeedX > 0) {
//                     CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, ReleaseDecelerationX, 0);
//                 }
//
//                 // If change direction, skid until lose all momentum then turn around
//                 if (IsChangingDirection) {
//                     if (CurrentSpeedX > SkidTurnaroundSpeedX) {
//                         MoveDirectionX = -FaceDirectionX;
//                         MAnimator.SetBool(IsSkiddingAnimator, true);
//                         CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, SkidDecelerationX, 0);
//                     } else {
//                         MoveDirectionX = FaceDirectionX;
//                         MAnimator.SetBool(IsSkiddingAnimator, false);
//                     }
//                 } else {
//                     MAnimator.SetBool(IsSkiddingAnimator, false);
//                 }
//
//                 /******** Horizontal movement on air */
//             } else {
//                 SetMidairParams();
//
//                 // Holding Dash while in midair has no effect
//                 if (FaceDirectionX != 0) {
//                     switch (CurrentSpeedX) {
//                         case 0:
//                             CurrentSpeedX = MinWalkSpeedX;
//                             break;
//                         case < MaxWalkSpeedX:
//                             CurrentSpeedX = IncreaseWithinBound(CurrentSpeedX, midairAccelerationX, MaxWalkSpeedX);
//                             break;
//                         default:
//                         {
//                             if (WasDashingBeforeJump && CurrentSpeedX < MaxRunSpeedX) {
//                                 CurrentSpeedX = IncreaseWithinBound(CurrentSpeedX, midairAccelerationX, MaxRunSpeedX);
//                             }
//
//                             break;
//                         }
//                     }
//                 } else if (CurrentSpeedX > 0) {
//                     CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, ReleaseDecelerationX, 0);
//                 } else if (!PlayerControls.Player.Jump.triggered && !IsJumping && !IsFalling) {
//                     MRigidbody2D.gravityScale = NormalGravity * 1.0f; //jumpDownGravity; //1.0f;
//                 }
//
//                 // If change direction, decelerate but keep facing move direction
//                 if (IsChangingDirection) {
//                     IsChangedDirOnAirYes = true;
//                     FaceDirectionX = MoveDirectionX;
//                     CurrentSpeedX = DecreaseWithinBound(CurrentSpeedX, midairDecelerationX, 0);
//                 }
//             }
//
//             // Disable Stomp Box if not falling down
//             MStompBox.SetActive(IsFalling);
//
//             Transform transformCached = transform;
//             transformCached.localScale = FaceDirectionX switch {
//                 /******** Horizontal orientation */
//                 > 0 => Vector2.one,
//                 < 0 => new Vector2(-1, 1),
//                 _ => transformCached.localScale
//             };
//
//             // /******** Reset params for automatic movement */
//             // if (InputFreezed) {
//             // 	CurrentSpeedX = AutomaticWalkSpeedX;
//             // 	MRigidbody2D.gravityScale = AutomaticGravity;
//             // }
//
//             /******** Set params */
//             MRigidbody2D.velocity = new Vector2(MoveDirectionX * CurrentSpeedX, MRigidbody2D.velocity.y);
//
//             //MAnimator.SetBool(IsJumpingAnimator, IsJumping);
//             //MAnimator.SetBool(IsFallingNotFromJumpAnimator, IsFalling && !IsJumping);
//             //MAnimator.SetBool(IsCrouchingAnimator, IsCrouching);
//             //MAnimator.SetFloat(AbsSpeedAnimator, Mathf.Abs(CurrentSpeedX));
//
//             if (FaceDirectionX != 0 && !IsChangingDirection) {
//                 MoveDirectionX = FaceDirectionX;
//             }
//         }
//
//         /****************** Movement control */
//         private void SetJumpParams() {
//             switch (CurrentSpeedX) {
//                 case < 3.75f:
//                     jumpSpeedY = 15f;
//                     jumpUpGravity = .47f;
//                     jumpDownGravity = 1.64f;
//                     break;
//                 case < 8.67f:
//                     jumpSpeedY = 15f;
//                     jumpUpGravity = .44f;
//                     jumpDownGravity = 1.41f;
//                     break;
//                 default:
//                     jumpSpeedY = 18.75f;
//                     jumpUpGravity = .59f;
//                     jumpDownGravity = 2.11f;
//                     break;
//             }
//         }
//
//         private void SetMidairParams() {
//             if (CurrentSpeedX < 5.86f) {
//                 midairAccelerationX = .14f;
//                 midairDecelerationX = SpeedXBeforeJump < 6.80f ? .14f : .19f;
//             } else {
//                 midairAccelerationX = .21f;
//                 midairDecelerationX = .21f;
//             }
//         }
//
//         public bool isDying;
//
//         private float deadUpTimer = .25f;
//
// //	Vector2 deadUpVelocity = new Vector2 (0, 10f);
// //	Vector2 deadDownVelocity = new Vector2 (0, -15f);
//         public void FreezeAndDie() {
//             FreezeUserInput();
//             isDying = true;
//             MRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
//             MAnimator.SetTrigger(RespawnAnimator);
//             gameObject.layer = LayerMask.NameToLayer("Falling to Kill Plane");
//             gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground Effect";
//         }
//
//
//         private bool isClimbingFlagPole = false;
//         private readonly Vector2 climbFlagPoleVelocity = new Vector2(0, -5f);
//         private static readonly int MarioSizeAnimator = Animator.StringToHash("marioSize");
//         private static readonly int PoleAnimator = Animator.StringToHash("climbFlagPole");
//         private static readonly int RespawnAnimator = Animator.StringToHash("respawn");
//         private static readonly int IsJumpingAnimator = Animator.StringToHash("isJumping");
//         private static readonly int IsFallingNotFromJumpAnimator = Animator.StringToHash("isFallingNotFromJump");
//         private static readonly int IsCrouchingAnimator = Animator.StringToHash("isCrouching");
//         private static readonly int AbsSpeedAnimator = Animator.StringToHash("absSpeed");
//         private static readonly int IsFiringAnimator = Animator.StringToHash("isFiring");
//         private static readonly int IsSkiddingAnimator = Animator.StringToHash("isSkidding");
//
//         public void ClimbFlagPole() {
//             FreezeUserInput();
//             isClimbingFlagPole = true;
//             MAnimator.SetBool(PoleAnimator, true);
//             MRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
//             Debug.Log(this.name + ": Mario starts climbing flag pole");
//         }
//
//
//         private void JumpOffPole() {
//             // get off pole and start walking right
//             Transform transformCached = transform;
//             Vector3 position = transformCached.position;
//             position = new Vector2(position.x + .5f, position.y);
//             transformCached.position = position;
//             MAnimator.SetBool(PoleAnimator, false);
//             AutomaticWalk(CastleWalkSpeedX);
//             MRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
//             Debug.Log(this.name + ": Mario jumps off pole and walks to castle");
//         }
//
//
//         /****************** Automatic movement (e.g. walk to castle sequence) */
//         public void UnfreezeUserInput() {
//             InputFreezed = false;
//             Debug.Log(this.name + " UnfreezeUserInput called");
//         }
//
//         public void FreezeUserInput() {
//             InputFreezed = true;
//             InputFreeze?.Invoke();
//             JumpButtonHeld = false;
//
//             FaceDirectionX = 0;
//             MoveDirectionX = 0;
//
//             CurrentSpeedX = 0;
//             SpeedXBeforeJump = 0;
//             AutomaticWalkSpeedX = 0;
//             AutomaticGravity = NormalGravity;
//
//             IsDashing = false;
//             WasDashingBeforeJump = false;
//             IsCrouching = false;
//             IsChangingDirection = false;
//             IsShooting = false;
//
//             gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero; // stop all momentum
//             Debug.Log(this.name + " FreezeUserInput called");
//         }
//
//
//         public void AutomaticWalk(float walkVelocityX) {
//             FreezeUserInput();
//             if (walkVelocityX != 0) {
//                 FaceDirectionX = walkVelocityX / Mathf.Abs(walkVelocityX);
//             }
//
//             AutomaticWalkSpeedX = Mathf.Abs(walkVelocityX);
//             Debug.Log(this.name + " AutomaticWalk: speed=" +
//                       AutomaticWalkSpeedX.ToString(CultureInfo.InvariantCulture));
//         }
//
//
//         public void AutomaticCrouch() {
//             FreezeUserInput();
//             IsCrouching = true;
//         }
//
//
//         /****************** Misc */
//         public void UpdateSize() {
//             GetComponent<Animator>().SetInteger(MarioSizeAnimator, FindObjectOfType<global::LevelManager>().marioSize);
//         }
//
//         private static float IncreaseWithinBound(float val, float delta, float maxVal = Mathf.Infinity) {
//             val += delta;
//             if (val > maxVal) {
//                 val = maxVal;
//             }
//
//             return val;
//         }
//
//         private static float DecreaseWithinBound(float val, float delta, float minVal = 0) {
//             val -= delta;
//             if (val < minVal) {
//                 val = minVal;
//             }
//
//             return val;
//         }
//
//         private void OnCollisionEnter2D(Collision2D other) {
//             // GetComponent<MarioStompBox>().
//             if (MStompBox.activeSelf && MStompBox.GetComponent<Collider2D>().isTrigger)
//                 return; // ignore collisions with stomp box
//             Vector2 normal = other.contacts[0].normal;
//             Vector2 bottomSide = new Vector2(0f, 1f);
//             bool bottomHit = normal == bottomSide;
//
//             if (other.gameObject.tag.Contains("Enemy")) {
//                 // TODO: koopa shell static does no damage
//                 Enemy enemy = other.gameObject.GetComponent<Enemy>();
//
//                 if (!LevelManager.isInvincible()) {
//                     if (!other.gameObject.GetComponent<KoopaShell>() ||
//                         other.gameObject.GetComponent<KoopaShell>()
//                             .isRolling || // non-rolling shell should do no damage
//                         !bottomHit || (!enemy.isBeingStomped)) {
//                         Debug.Log(this.name + " OnCollisionEnter2D: Damaged by " + other.gameObject.name
//                                   + " from " + normal.ToString() + "; isFalling=" +
//                                   IsFalling); // TODO sometimes fire before stompbox reacts
//                         LevelManager.MarioPowerDown();
//                     }
//                 } else if (LevelManager.isInvincibleStarman) {
//                     LevelManager.MarioStarmanTouchEnemy(enemy);
//                 }
//             } else if (other.gameObject.CompareTag("Goal") && isClimbingFlagPole && bottomHit) {
//                 Debug.Log(this.name + ": Mario hits bottom of flag pole");
//                 isClimbingFlagPole = false;
//                 JumpOffPole();
//             }
//         }
//     }
// }