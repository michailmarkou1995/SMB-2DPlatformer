using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    //Following Variables are responsible for movement
    [SerializeField] private float moveSpeed = 18;
    private float faceDirectionX, movement;
    private bool isChangingDirection;
    private bool isMoving;
    private bool isJumping = false;
    private Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        //This is a comment test
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        faceDirectionX = Input.GetAxisRaw("Horizontal") * moveSpeed; // > 0 for right, < 0 for left

        if (Input.GetKey(KeyCode.Space) && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, moveSpeed / 2);
        }
        rb.velocity = new Vector2(faceDirectionX * movement, rb.velocity.y);
        /******** Horizontal orientation */
        if (faceDirectionX > 0)
        {
            transform.localScale = new Vector2(1, 1); // facing right
        }
        else if (faceDirectionX < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        /**animation**/ //This is where we will set the animation parameters
        anim.SetBool("isJumping", isJumping);
        anim.SetFloat("absSpeed", Mathf.Abs(rb.velocity.x));
    }

    private void FixedUpdate()
    {
        //FixedUpdate is used in conjuction with Time.deltaTime to make movement independent of frame rate
        movement = moveSpeed * Time.deltaTime;
    }

    //Checks to see if the player is colliding with something and deems the player as "grounded", allowing them to jump.
    void OnCollisionStay2D(Collision2D col)
    {
        isJumping = false;
    }

    //Checks to see if the player is jumping
    void OnCollisionExit2D(Collision2D col)
    {
        isJumping = true;
    }
}