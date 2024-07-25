using UnityEngine;

public class UserController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveVelocity;
    private Animator animator;
    private PlayerStats playerStats;
    private SpriteRenderer spriteRenderer;

    private Vector2 facingDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * playerStats.GetCurrentMoveSpeed();

        // Handle animations
        if (moveInput != Vector2.zero)
        {
            animator.SetBool("move", true);
            facingDirection = moveInput.normalized; // Update facing direction when moving
        }
        else
        {
            animator.SetBool("move", false);
        }

        // Handle sprite flipping
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }
}
