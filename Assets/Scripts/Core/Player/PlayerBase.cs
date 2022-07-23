using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Player
{
    //internal static class IsExternalInit { }

    public abstract class PlayerBase : MonoBehaviour
    {
        protected global::Core.Managers.LevelManager LevelManager { get; private set; }
        protected Transform MGroundCheck1 { get; private set; }
        protected Transform MGroundCheck2 { get; private set; }
        protected CapsuleCollider2D MCapsuleCollider2D { get; private set; }
        protected GameObject MStompBox { get; private set; }
        protected Animator MAnimator { get; private set; }
        protected Rigidbody2D MRigidbody2D { get; private set; }

        // Player Controls
        protected PlayerInputActions PlayerControls { get; set; }

        protected int GroundLayers { get; private set; }

        [FormerlySerializedAs("GroundLayers")] public LayerMask groundLayers;
        [FormerlySerializedAs("Fireball")] public GameObject fireball;
        [FormerlySerializedAs("FirePos")] public Transform firePos;
        protected bool IsClimbingFlagPole { get; set; }
        protected Vector2 ClimbFlagPoleVelocity { get; } = new(0, -10f);

        // Exposed to Editor explicit backing fields of properties Form
        [Header("Animator Name Parameters")] [Tooltip("Default value: playerSize")] [SerializeField]
        private string playerSizeAnimator = "playerSize";

        [Tooltip("Default value: climbFlagPole")] [SerializeField]
        private string poleAnimator = "climbFlagPole";

        [Tooltip("Default value: respawn")] [SerializeField]
        private string respawnAnimator = "respawn";

        [Tooltip("Default value: isJumping")] [SerializeField]
        private string isJumpingAnimator = "isJumping";

        [Tooltip("Default value: isFallingNotFromJump")] [SerializeField]
        private string isFallingNotFromJumpAnimator = "isFallingNotFromJump";

        [Tooltip("Default value: isCrouching")] [SerializeField]
        private string isCrouchingAnimator = "isCrouching";

        [Tooltip("Default value: absSpeed")] [SerializeField]
        private string absSpeedAnimator = "absSpeed";

        [Tooltip("Default value: isFiring")] [SerializeField]
        private string isFiringAnimator = "isFiring";

        [Tooltip("Default value: isSkidding")] [SerializeField]
        private string isSkiddingAnimator = "isSkidding";

        // getters for exposed backing fields
        protected int PlayerSizeAnimator => Animator.StringToHash(playerSizeAnimator);

        protected int PoleAnimator => Animator.StringToHash(poleAnimator);

        protected int RespawnAnimator => Animator.StringToHash(respawnAnimator);

        protected int IsJumpingAnimator => Animator.StringToHash(isJumpingAnimator);

        protected int IsFallingNotFromJumpAnimator => Animator.StringToHash(isFallingNotFromJumpAnimator);

        protected int IsCrouchingAnimator => Animator.StringToHash(isCrouchingAnimator);

        protected int AbsSpeedAnimator => Animator.StringToHash(absSpeedAnimator);

        protected int IsFiringAnimator => Animator.StringToHash(isFiringAnimator);

        protected int IsSkiddingAnimator => Animator.StringToHash(isSkiddingAnimator);


        // Events
        protected event Action InputFreeze;

        // Player Actions Constant Variables
        //TODO make interface
        protected const float WaitBetweenFire = .2f;
        protected const float MinWalkSpeedX = .28f;
        protected const float WalkAccelerationX = .14f;
        protected const float RunAccelerationX = .21f;
        protected const float ReleaseDecelerationX = .25f; // original: .19f;
        protected const float SkidDecelerationX = .5f; // .38f;
        protected const float SkidTurnaroundSpeedX = 3.5f; // 2.11;
        protected const float MaxWalkSpeedX = 5.86f;
        protected const float MaxRunSpeedX = 9.61f;

        protected float JumpSpeedY { get; set; }
        protected float JumpUpGravity { get; set; }
        protected float JumpDownGravity { get; set; }
        protected float MidairAccelerationX { get; set; }
        protected float MidairDecelerationX { get; set; }

        protected float FireTime1 { get; set; }
        protected float FireTime2 { get; set; }
        protected float FaceDirectionX { get; set; }
        protected float MoveDirectionX { get; set; }
        protected float NormalGravity { get; private set; }

        protected float CurrentSpeedX { get; set; }
        protected float SpeedXBeforeJump { get; set; }

        protected float AutomaticWalkSpeedX { get; set; }
        protected float AutomaticGravity { get; set; }

        // Walk Speed Form
        [Header("Walk Speed")] [SerializeField]
        private float castleWalkSpeedX = 5.86f;

        [SerializeField] private float levelEntryWalkSpeedX = 3.05f;

        public float CastleWalkSpeedX => castleWalkSpeedX;

        public float LevelEntryWalkSpeedX => levelEntryWalkSpeedX;

        protected bool IsGrounded { get; set; }
        protected bool IsDashing { get; set; }
        protected bool IsFalling { get; set; }
        protected bool IsJumping { get; set; }
        protected bool IsChangingDirection { get; set; }
        protected bool WasDashingBeforeJump { get; set; }
        protected bool IsShooting { get; set; }
        public bool IsCrouching { get; protected set; }
        protected bool IsChangedDirOnAirYes { get; set; }
        protected bool JumpButtonHeld { get; set; }
        protected bool InputFreezed { get; set; }

        protected virtual void OnInputFreeze()
        {
            InputFreeze?.Invoke();
        }

        /// <summary>
        /// Run on Awake to initialize the player and get references to components.<br/>
        /// Set Default Gravity permanently.<br/>
        /// Allowed for override in child classes.
        /// </summary>
        protected virtual void InitializationComponents()
        {
            LevelManager = FindObjectOfType<global::Core.Managers.LevelManager>();
            MGroundCheck1 = transform.Find("Ground Check 1");
            MGroundCheck2 = transform.Find("Ground Check 2");
            MStompBox = transform.Find("Stomp Box").gameObject;
            MCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
            GetComponent<BoxCollider2D>();
            MAnimator = GetComponent<Animator>();
            MRigidbody2D = GetComponent<Rigidbody2D>();
            NormalGravity = MRigidbody2D.gravityScale;


            #region Default_AnimatorParams

            // string sizeAnimatorLocal = playerSizeAnimator == null
            //     ? playerSizeAnimator = "playerSize"
            //     : playerSizeAnimator.Equals(string.Empty)
            //         ? playerSizeAnimator = "playerSize"
            //         : playerSizeAnimator;
            // PlayerSizeAnimator = Animator.StringToHash(sizeAnimatorLocal);
            //
            // poleAnimator ??= "climbFlagPole";
            // string climbFlagpoleLocal = poleAnimator.Equals(string.Empty)
            //     ? poleAnimator = "climbFlagPole"
            //     : poleAnimator;
            // PoleAnimator = Animator.StringToHash(climbFlagpoleLocal);
            //
            // respawnAnimator ??= "respawn";
            // string respawnLocal = respawnAnimator.Equals(string.Empty)
            //     ? respawnAnimator = "respawn"
            //     : respawnAnimator;
            // RespawnAnimator = Animator.StringToHash(respawnLocal);
            //
            // isJumpingAnimator ??= "isJumping";
            // string isJumpingLocal = isJumpingAnimator.Equals(string.Empty)
            //     ? isJumpingAnimator = "isJumping"
            //     : isJumpingAnimator;
            // IsJumpingAnimator = Animator.StringToHash(isJumpingLocal);
            //
            // isFallingNotFromJumpAnimator ??= "isFallingNotFromJump";
            // string isFallingNotFromJumpLocal = isFallingNotFromJumpAnimator.Equals(string.Empty)
            //     ? isFallingNotFromJumpAnimator = "isFallingNotFromJump"
            //     : isFallingNotFromJumpAnimator;
            // IsFallingNotFromJumpAnimator = Animator.StringToHash(isFallingNotFromJumpLocal);
            //
            // isCrouchingAnimator ??= "isCrouching";
            // string isCrouchingLocal = isCrouchingAnimator.Equals(string.Empty)
            //     ? isCrouchingAnimator = "isCrouching"
            //     : isCrouchingAnimator;
            // IsCrouchingAnimator = Animator.StringToHash(isCrouchingLocal);
            //
            // absSpeedAnimator ??= "absSpeed";
            // string absSpeedLocal = absSpeedAnimator.Equals(string.Empty)
            //     ? absSpeedAnimator = "absSpeed"
            //     : absSpeedAnimator;
            // AbsSpeedAnimator = Animator.StringToHash(absSpeedLocal);
            //
            // isFiringAnimator ??= "isFiring";
            // string isFiringLocal = isFiringAnimator.Equals(string.Empty)
            //     ? isFiringAnimator = "isFiring"
            //     : isFiringAnimator;
            // IsFiringAnimator = Animator.StringToHash(isFiringLocal);
            //
            // isSkiddingAnimator ??= "isSkidding";
            // string isSkiddingLocal = isSkiddingAnimator.Equals(string.Empty)
            //     ? isSkiddingAnimator = "isSkidding"
            //     : isSkiddingAnimator;
            // IsSkiddingAnimator = Animator.StringToHash(isSkiddingLocal);

            #endregion

            if (PlayerSizeAnimator == 0) throw new NotImplementedException();
            if (PoleAnimator == 0) throw new NotImplementedException();
            if (RespawnAnimator == 0) throw new NotImplementedException();
            if (IsJumpingAnimator == 0) throw new NotImplementedException();
            if (IsFallingNotFromJumpAnimator == 0) throw new NotImplementedException();
            if (IsCrouchingAnimator == 0) throw new NotImplementedException();
            if (AbsSpeedAnimator == 0) throw new NotImplementedException();
            if (IsFiringAnimator == 0) throw new NotImplementedException();
            if (IsSkiddingAnimator == 0) throw new NotImplementedException();

            if (firePos == null) throw new NullReferenceException();
            if (fireball == null) throw new NullReferenceException();


            #region Default_GroundLayerMask

            // if (groundLayers.Equals(default(LayerMask))) {
            //     // Player Interaction with other Layers e.g., used in raycasts to determine if is jumping or falling.
            //     GroundLayers = (1 << LayerMask.NameToLayer("Ground")
            //                     | 1 << LayerMask.NameToLayer("Block")
            //                     | 1 << LayerMask.NameToLayer("Goal")
            //                     | 1 << LayerMask.NameToLayer("Player Detector")
            //                     | 1 << LayerMask.NameToLayer("Moving Platform"));
            // } else {
            //     GroundLayers = groundLayers;
            // }

            #endregion

            // check struct on default uninitialized value then if it is null, throw exception
            // Player Interaction with other Layers e.g., used in raycasts to determine if is jumping or falling.
            GroundLayers = groundLayers.Equals(default(LayerMask))
                ? throw new NotImplementedException()
                : groundLayers;
        }

        /// <summary>
        /// Register Players Input System.<br/>
        /// Must be implemented on each child class.
        /// </summary>
        protected abstract void PlayerControlsSubscribe();

        /// <summary>
        /// Run on Start to initialize the player and get references to components on each load level.<br/>
        /// Must be implemented on each child class.
        /// </summary>
        protected abstract void PerLevelInitialization();


        /// <summary>
        /// Change Size of Player as a power-up.<br/>
        /// Allowed for override in child classes but if called without then exception is thrown.
        /// </summary>
        public virtual void UpdateSize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Animation Parameters for Animation Controller with clips embedded.<br/>
        /// Must be implemented on each child class.
        /// </summary>
        protected abstract void AnimationParams();
    }
}