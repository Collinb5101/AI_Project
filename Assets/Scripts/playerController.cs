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

    public float speed = 5;
    public float jumpSpeed = 2;
    private float horizontalInput;
    private float verticalInput;

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
        verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector2.right * Time.deltaTime * speed * horizontalInput);
        transform.Translate(Vector2.up * Time.deltaTime * speed * verticalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody2D>().velocity += jumpSpeed * Vector2.up;
        }
    }
}
