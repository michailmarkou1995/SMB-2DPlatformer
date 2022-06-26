using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Petros : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    //Following Variables are responsible for movement
    private float maxSpeed;
    private float minSpeed = 14;
    private float moveSpeed;
    private float moveHorizontal;

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

        if (Input.GetKey(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, moveSpeed);
        }
    }

    private void FixedUpdate()
    {
        float movement = moveSpeed * Time.deltaTime;
        rb.velocity = new Vector2(moveHorizontal* movement, rb.velocity.y);
    }
}
