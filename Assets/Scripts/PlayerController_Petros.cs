using UnityEngine;

public class PlayerController_Petros : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    //Following Variables are responsible for movement
    [SerializeField] private float moveSpeed = 18;
    private float moveHorizontal;

    private bool isJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (Input.GetKey(KeyCode.Space) && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, moveSpeed / 2);
        }
    }

    private void FixedUpdate()
    {
        //FixedUpdate is used in conjuction with Time.deltaTime to make movement independent of frame rate
        float movement = moveSpeed * Time.deltaTime;
        rb.velocity = new Vector2(moveHorizontal * movement, rb.velocity.y);
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