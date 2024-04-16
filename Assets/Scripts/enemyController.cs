using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class enemyController : MonoBehaviour
{
    

    [Header("AI")]
    private Transform target;
    public Transform playerLocation;
    public Transform playerAnticipatorLocation;
    public Transform fleeLocation;
    public float activationDistance;
    public float pathUpdateSpeed;

    [Header("Physics")]
    public float speed;
    public float nextWaypointDistance;
    public float jumpNodeHeight;
    public float jumpHeight; 
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;

    [Header("Behavior")]
    public bool isFollowing;
    public bool jumpEnabled;

    private Path path;
    private int currentWaypoint;
    private bool isGrounded;
    private Seeker seeker;
    Rigidbody2D rb;

    private float timer;
    private Vector2 lastPosition;
    private bool hiding = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(Random.Range(-33, 33), Random.Range(-33, 33));
        lastPosition = transform.position;

        seeker = GetComponent<Seeker>();    
        rb = GetComponent<Rigidbody2D>();
        target = playerAnticipatorLocation;

        InvokeRepeating("UpdatePath", 0f, pathUpdateSpeed);
    }
    private void FixedUpdate()
    {
        if(target == fleeLocation)
        {
            if(Vector2.Distance(transform.position, target.position) <= 5 && !hiding)
            {
                hiding = true;
                timer = 0;
            }
        }
        else if (Vector2.Distance(transform.position, playerLocation.position) <= activationDistance)
        {
            target = playerLocation;
        }
        else
        {
           target = playerAnticipatorLocation;
        }

        if(!hiding)
        {
            FollowTarget();
        }
        

        timer += Time.deltaTime;

        if(timer >= 3 && !hiding)
        {
            if(Vector2.Distance(lastPosition, transform.position) <= 3)
            {
                transform.position = new Vector2(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(-1, 1));
            }

            lastPosition = transform.position;
            timer = 0;
        }
        else if (timer >= 3 && hiding)
        {
            hiding = false;
            target = playerAnticipatorLocation;
        }
    }

    private void UpdatePath()
    {
        if(isFollowing&& seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);

        }
    }

    private void FollowTarget()
    {
        if(path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        isGrounded = Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
    
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint]-rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        if (jumpEnabled && isGrounded)
        {
            if(direction.y > jumpNodeHeight)
            {
                rb.AddForce(Vector2.up * jumpHeight);
            }
        }

        rb.AddForce(force);
        
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }


    private void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            this.path = path;
            currentWaypoint = 0;
        }
    }
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == playerLocation.gameObject)
        {
            fleeLocation.position = new Vector2(Random.Range(-33, 33), Random.Range(-33, 33));
            target = fleeLocation;
        }
    }
}
