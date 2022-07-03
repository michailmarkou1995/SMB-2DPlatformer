using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class PlayerController : MonoBehaviour
    {
        // Inspector Components
        private Rigidbody2D rb;
        private BoxCollider2D bc;

        // Player Controls
        // or if not file generated then public InputAction pMove, InputAction pJump or as component to player
        private PlayerInputActions playerControls;

        // Animator state machine with animation clips attached to it
        private Animator anim;

        // Horizontal Movement
        private const float MoveSpeed = 18.0f;
        private float faceDirectionX, movement;
        private bool isChangingDirection;
        private bool isMoving;
        private bool isJumping;

        // Cached Animator Parameters
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private static readonly int AbsSpeed = Animator.StringToHash("absSpeed");

        // Singleton Pattern
        private static PlayerController instance = null;

        public static PlayerController Instance
        {
            // can be read by other scripts, but it can only be set from within its own class.
            get => instance;
            private set => instance = value;
        }


        // Start is called before the first frame update
        private void Awake()
        {
            // If there is not already an instance of PlayerController, set it to this.
            if (Instance == null)
            {
                Instance = this;
            }
            //If an instance already exists, destroy whatever this object is to enforce the singleton.
            else if (Instance != this) // != null
            {
                Destroy(gameObject); // Destroy(this)
            }

            //Set PlayerController to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad(gameObject);

            playerControls = new PlayerInputActions(); // you could either attach it as a component or create here a new object
            playerControls.Player.Move.performed +=
                ctx => faceDirectionX = ctx.ReadValue<float>() * MoveSpeed; // > 0 for right, < 0 for left
            playerControls.Player.Move.canceled += ctx => faceDirectionX = 0;

            //playerControls.Player.Jump.performed += Jump_performed;
            rb = GetComponent<Rigidbody2D>();
            bc = GetComponent<BoxCollider2D>();
            anim = GetComponent<Animator>();
        }

        // private void Jump_performed(InputAction.CallbackContext context)
        // {
        //     Debug.Log(context);
        //     if (playerControls.Player.Jump.triggered && !isJumping)
        //     {
        //         rb.velocity = new Vector2(rb.velocity.x, MoveSpeed / 2.0f);
        //     }
        // }

        private void OnEnable()
        {
            playerControls.Player.Enable();
        }

        private void OnDisable()
        {
            playerControls.Player.Disable();
        }

        // Update is called once per frame
        private void Update()
        {
            
            if (playerControls.Player.Jump.triggered && !isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, MoveSpeed / 2.0f);
            }
            
            rb.velocity = new Vector2(faceDirectionX * movement, rb.velocity.y);

            var transformCached = transform;

            // Change Mario's facing direction if he is not already changing direction
            transformCached.localScale = faceDirectionX switch
            {
                /******** Horizontal orientation */
                > 0 => Vector2.one,
                < 0 => new Vector2(-1, 1),
                _ => transformCached.localScale
            };

            // This is where we will set the animation parameters
            anim.SetBool(IsJumping, isJumping);
            anim.SetFloat(AbsSpeed, Mathf.Abs(rb.velocity.x));
        }

        private void FixedUpdate()
        {
            // FixedUpdate is used in conjunction with Time.deltaTime to make movement independent of frame rate
            movement = MoveSpeed * Time.deltaTime;
        }

        // Checks to see if the player is colliding with something and deems the player as "grounded", allowing them to jump.
        private void OnCollisionStay2D(Collision2D col)
        {
            isJumping = false;
        }

        // Checks to see if the player is jumping
        private void OnCollisionExit2D(Collision2D col)
        {
            isJumping = true;
        }
    }
}