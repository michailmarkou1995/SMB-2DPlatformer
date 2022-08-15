using Core.NPC;
using Interfaces.Core.Managers;
using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        public GameObject MStompBox { get; set; }
        public Rigidbody2D MRigidbody2D { get; set; }
        public CapsuleCollider2D MCapsuleCollider2D { get; set; }
        public BoxCollider2D MBoxCollider2D { get; set; }
        public Transform MGroundCheck1 { get; set; }
        public Transform MGroundCheck2 { get; set; }
        public Collider2D[] Colliders1 { get; set; } = new Collider2D[1];
        public Collider2D[] Colliders2 { get; set; } = new Collider2D[1];

        #region GettersAndSetters

        //public IPlayerAnimator GetPlayerAnimator => _animator;

        public PlayerInputActions PlayerControls { get; set; }

        #endregion

        #region RequiredComponents

        public ILevelManager GetLevelManager => _levelManager;
        public IMove GetMovement => _move;
        public ICrouch GetCrouch => _crouch;
        public IJump GetJump => _jump;
        public IDash GetDash => _dash;
        public IAttack GetAttack => _attack;
        public IMovementFreeze GetMovementFreeze => _movementFreeze;

        public IGroundCheck GetGroundCheck => _groundCheck;

        public IPlayerAnimationParams GetAnimationParams => _animationParams;

        public IPlayerSize GetPlayerSize => _playerSize;

        public IDeath GetDeath => _death;
        //private IPlayerAnimator _animator;

        #endregion

        private ILevelManager _levelManager;
        private IMove _move;
        private IDash _dash;
        private IJump _jump;
        private ICrouch _crouch;
        private IAttack _attack;
        private IMovementFreeze _movementFreeze;
        private IGroundCheck _groundCheck;
        private IPlayerAnimationParams _animationParams;
        private IPlayerSize _playerSize;
        private IDeath _death;

        private void Awake()
        {
            PlayerControlsSubscribe();
            InitializationComponents();
        }

        public void PlayerControlsSubscribe()
        {
            //_animator = GetComponent<IPlayerAnimator>();
            _levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<ILevelManager>();
            _move = GetComponent<IMove>();
            _dash = GetComponent<IDash>();
            _jump = GetComponent<IJump>();
            _crouch = GetComponent<ICrouch>();
            _attack = GetComponent<IAttack>();
            _movementFreeze = GetComponent<IMovementFreeze>();
            _groundCheck = GetComponent<IGroundCheck>();
            _animationParams = GetComponent<IPlayerAnimationParams>();
            _playerSize = GetComponent<IPlayerSize>();
            _death = GetComponent<IDeath>();

            PlayerControls = new PlayerInputActions();

            PlayerControls.Player.Move.performed += ctx => _move.FaceDirectionX = ctx.ReadValue<float>();
            PlayerControls.Player.Move.canceled += ctx => _move.FaceDirectionX = 0;

            PlayerControls.Player.Crouch.performed += _crouch.Crouch_performed;
            PlayerControls.Player.Crouch.canceled += _crouch.Crouch_canceled;

            PlayerControls.Player.Jump.performed += _jump.Jump_performed;
            PlayerControls.Player.Jump.canceled += _jump.Jump_canceled;

            PlayerControls.Player.Dash.performed += ctx => _dash.IsDashing = true;
            PlayerControls.Player.Dash.canceled += ctx => _dash.IsDashing = false;

            PlayerControls.Player.Fire.started += ctx => _attack.IsShooting = true;
            PlayerControls.Player.Fire.performed += _attack.Shooting;
            PlayerControls.Player.Fire.canceled += ctx => _attack.IsShooting = false;
        }

        public void InitializationComponents()
        {
            _levelManager = FindObjectOfType<global::Core.Managers.LevelManager>();
            MGroundCheck1 = transform.Find("Ground Check 1");
            MGroundCheck2 = transform.Find("Ground Check 2");
            MStompBox = transform.Find("Stomp Box").gameObject;
            MCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
            MBoxCollider2D = GetComponent<BoxCollider2D>();
            GetComponent<BoxCollider2D>();
            PlayerAnimatorStatic.PlayerAnimatorComponent = GetComponent<Animator>();
            MRigidbody2D = GetComponent<Rigidbody2D>();
            _jump.NormalGravity = MRigidbody2D.gravityScale;
        }

        private void Start()
        {
            PerLevelInitialization();
        }

        private void OnEnable()
        {
            PlayerControls.Player.Enable();
            _movementFreeze.InputFreeze += _move.ResetMovementParams;
        }

        private void OnDisable()
        {
            PlayerControls.Player.Disable();
            _movementFreeze.InputFreeze -= _move.ResetMovementParams;
        }

        private void Update()
        {
            // Debug.Log("isGrounded: " + _groundCheck.IsGrounded
            //                          + " isJumping: " + _jump.IsJumping
            //                          + " isFalling: " + _jump.IsFalling);
            _groundCheck.IsGrounded = _groundCheck.IsGround();
            _jump.IsFalling = MRigidbody2D.velocity.y < 0 && !_groundCheck.IsGrounded;
            _move.IsChangingDirection = _move.CurrentSpeedX > 0 && _move.FaceDirectionX * _move.MoveDirectionX < 0;

            if (!_movementFreeze.InputFreezed || _levelManager.GetGameStateData.GamePaused) return;
            if (_death.IsDying) {
                _death.DeadUpTimer -= Time.unscaledDeltaTime;
                if (_death.DeadUpTimer > 0) {
                    // TODO MovePosition not working
//					m_Rigidbody2D.MovePosition (m_Rigidbody2D.position + deadUpVelocity * Time.unscaledDeltaTime);
                    gameObject.transform.position += Vector3.up * .22f;
                } else {
//					m_Rigidbody2D.MovePosition (m_Rigidbody2D.position + deadDownVelocity * Time.unscaledDeltaTime);
                    gameObject.transform.position += Vector3.down * .2f;
                }
            } else if (_move.IsClimbingFlagPole) {
                MRigidbody2D.MovePosition(MRigidbody2D.position + _move.ClimbFlagPoleVelocity * Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            _move.Movement();
            _animationParams.MovementAnimationParams();
        }

        private void PerLevelInitialization()
        {
            // Drop Mario at spawn position
            transform.position = FindObjectOfType<global::Core.Managers.LevelManager>()
                .GetLevelServices.FindSpawnPosition();

            // Set correct size
            _playerSize.UpdateSize();

            _attack.FireTime1 = 0;
            _attack.FireTime2 = 0;
            MStompBox.SetActive(false);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // ignore collisions with stomp box enabled .. stomping enemy should do no damage
            if (MStompBox.activeSelf && MStompBox.GetComponent<Collider2D>().isTrigger) return;
            //Debug.Log("Collision with " + other.gameObject.name);
            // MCapsuleCollider2D.enabled = other.gameObject.CompareTag("Pipe");

            Vector2 normal = other.contacts[0].normal;
            Vector2 bottomSide = new(0f, 1f);
            bool bottomHit = normal == bottomSide;

            if (other.gameObject.tag.Contains("Enemy")) {
                // TODO: koopa shell static does no damage
                Enemy enemy = other.gameObject.GetComponent<Enemy>();

                if (!_levelManager.GetPlayerAbilities.IsInvincible()) {
                    if (!other.gameObject.GetComponent<KoopaShell>() ||
                        other.gameObject.GetComponent<KoopaShell>()
                            .isRolling || // non-rolling shell should do no damage
                        !bottomHit || (!enemy.isBeingStomped)) {
                        _levelManager.GetPlayerAbilities.MarioPowerDown();
                    }
                } else if (_levelManager.GetPlayerAbilities.IsInvincibleStarman) {
                    _levelManager.GetPlayerAbilities.MarioStarmanTouchEnemy(enemy);
                }
            } else if (other.gameObject.CompareTag("Goal") && _move.IsClimbingFlagPole && bottomHit) {
                _move.IsClimbingFlagPole = false;
                _jump.JumpOffPole();
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

            if (!_levelManager.GetPlayerAbilities.IsInvincible()) {
                if (!collision.gameObject.GetComponent<KoopaShell>() ||
                    collision.gameObject.GetComponent<KoopaShell>()
                        .isRolling || // non-rolling shell should do no damage
                    !bottomHit || (!enemy.isBeingStomped)) {
                    _levelManager.GetPlayerAbilities.MarioPowerDown();
                }
            } else if (_levelManager.GetPlayerAbilities.IsInvincibleStarman) {
                _levelManager.GetPlayerAbilities.MarioStarmanTouchEnemy(enemy);
            }
        }
    }
}