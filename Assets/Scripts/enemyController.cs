using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class enemyController : MonoBehaviour
{
    [Header("AI")]
    public Transform target;
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

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();    
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSpeed);
    }
    private void FixedUpdate()
    {
        if(TargetInRange() && isFollowing)
        {
            FollowPlayer();
        }
    }

    private void UpdatePath()
    {
        if(isFollowing && TargetInRange() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);

        }
    }

    private void FollowPlayer()
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
                rb.AddForce(Vector2.up * speed * jumpHeight);
            }
        }

        rb.AddForce(force);
        
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private bool TargetInRange()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activationDistance;
    }

    private void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            this.path = path;
            currentWaypoint = 0;
        }
    }
}
