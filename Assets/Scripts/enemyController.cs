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
        //if the enemy is fleeing
        if(target == fleeLocation)
        {
            //if they've reached the flee location then they are hiding
            if(Vector2.Distance(transform.position, target.position) <= 5 && !hiding)
            {
                hiding = true;
                timer = 0;
            }
        }
        //if the player is within a certain distance of the enemy
        else if (Vector2.Distance(transform.position, playerLocation.position) <= activationDistance)
        {
            //target the player instead
            target = playerLocation;
        }
        else
        {
            //otherwise anticipate the player's future location
           target = playerAnticipatorLocation;
        }

        //if the enemy isnt hiding then follow the target
        if(!hiding)
        {
            FollowTarget();
        }
        
        //increase teh timer
        timer += Time.deltaTime;

        //check every 3 seconds as long as the enemy isnt hiding
        if(timer >= 3 && !hiding)
        {
            //if the enemy is within a certain distance of their last position
            if(Vector2.Distance(lastPosition, transform.position) <= 3)
            {
                //they are probably stuck, bump them in a random direction
                transform.position = new Vector2(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(-1, 1));
            }

            //update their last known position and reset the timer
            lastPosition = transform.position;
            timer = 0;
        }
        //if the enemy has been hiding for more than 3 seconds
        else if (timer >= 3 && hiding)
        {
            //start following the future position of teh player again
            hiding = false;
            target = playerAnticipatorLocation;
        }
    }

    private void UpdatePath()
    {
        //update the path for the enemy to follow
        if(isFollowing&& seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);

        }
    }

    private void FollowTarget()
    {
        //if the path is not valid
        if(path == null)
        {
            return;
        }

        //if the waypoint is out of range
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        //check for the enemy being grounded
        isGrounded = Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
    
        //calculate direction and movement
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint]-rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        //if the enemy can jump and is grounded
        if (jumpEnabled && isGrounded)
        {
            //and the node is high enough to be considered jumpable
            if(direction.y > jumpNodeHeight)
            {
                //jump
                rb.AddForce(Vector2.up * jumpHeight);
            }
        }

        //move
        rb.AddForce(force);
        
        //update the distance of the enemy to the next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }


    private void OnPathComplete(Path path)
    {
        //when the path is complete
        if (!path.error)
        {
            this.path = path;
            currentWaypoint = 0;
        }
    }
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //check for if the enemy has collided with the player
        if (collision.gameObject == playerLocation.gameObject)
        {
            //pick a random location to flee to and flee there
            fleeLocation.position = new Vector2(Random.Range(-33, 33), Random.Range(-33, 33));
            target = fleeLocation;
        }
    }
}
