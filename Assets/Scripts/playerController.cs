using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour
{
    Vector2 moveVector = Vector2.zero;
    Vector2 jumpVector = Vector2.zero;
    Rigidbody2D rBody;
    public float friction;

    public float speed;
    public float jumpForce;
    private float horizontalInput;
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;
    public float maxSpeed;

    public Transform playerAnticipator;

    // Start is called before the first frame update
    void Start()
    {
        moveVector = new Vector2(5, 0);
        jumpVector = new Vector2(0, 5);
        rBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        /*rBody.velocity += Vector2.right * speed * horizontalInput;
        
        if(rBody.velocity.x > maxSpeed)
        {
            rBody.velocity = new Vector2(maxSpeed, rBody.velocity.y);
        }
        if(rBody.velocity.x < -maxSpeed)
        {
            rBody.velocity = new Vector2(-maxSpeed, rBody.velocity.y);
        }*/
        //rBody.velocity += (Vector2.right * Time.deltaTime * speed * horizontalInput);
        transform.Translate(Vector2.right * Time.deltaTime * speed * horizontalInput);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            GetComponent<Rigidbody2D>().velocity += jumpForce * Vector2.up;
        }

        playerAnticipator.position = (Vector2)transform.position + /*rBody.velocity*/ moveVector*horizontalInput;
        /*
        if(horizontalInput == 0)
        {

            rBody.velocity *= friction;
        }*/
    }

    private bool IsGrounded()
    {
        if(Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }
}
