using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyStats enemyStats;
    private SpriteRenderer spriteRenderer;
    private Vector3 lastKnownPlayerPosition;
    private float nextTrackTime;

    public enum MovementType
    {
        FollowPlayer,
        RangedFollow,
        IntervalTrack
    }

    public MovementType movementType;
    public float stopDistance = 5f; // Distance for ranged enemies to keep
    public float trackInterval = 3f; // Interval for tracking player

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastKnownPlayerPosition = transform.position;
        nextTrackTime = Time.time;
    }

    void FixedUpdate()
    {
        switch (movementType)
        {
            case MovementType.FollowPlayer:
                MoveTowardsPlayer();
                break;
            case MovementType.RangedFollow:
                MoveTowardsPlayerRanged();
                break;
            case MovementType.IntervalTrack:
                IntervalTrackMovement();
                break;
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * enemyStats.moveSpeed * Time.fixedDeltaTime);

            // Set the moving animation
            animator.SetBool("Move", true);

            // Flip the sprite based on the movement direction
            FlipSprite(direction);
        }
    }

    private void MoveTowardsPlayerRanged()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > stopDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.MovePosition(rb.position + direction * enemyStats.moveSpeed * Time.fixedDeltaTime);

                // Set the moving animation
                animator.SetBool("Move", true);

                // Flip the sprite based on the movement direction
                FlipSprite(direction);
            }
            else
            {
                // Stop moving if within the stop distance
                animator.SetBool("Move", false);
            }
        }
    }

    private void IntervalTrackMovement()
    {
        if (player != null)
        {
            if (Time.time >= nextTrackTime)
            {
                lastKnownPlayerPosition = player.position;
                nextTrackTime = Time.time + trackInterval;
            }

            Vector2 direction = (lastKnownPlayerPosition - transform.position).normalized;
            rb.MovePosition(rb.position + direction * enemyStats.moveSpeed * Time.fixedDeltaTime);

            // Set the moving animation
            

            // Flip the sprite based on the movement direction
            FlipSprite(direction);

            // Face the player when not moving
            if (Time.time < nextTrackTime)
            {
                FacePlayer();
            }
        }
    }

    private void FlipSprite(Vector2 direction)
    {
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
    }

    private void FacePlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            FlipSprite(direction);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.health -= enemyStats.damage;
                Debug.Log("Player Health: " + playerStats.GetCurrentHealth());
            }

            // Optionally, you can also destroy the enemy upon collision
            // Destroy(gameObject);
        }
    }
}
