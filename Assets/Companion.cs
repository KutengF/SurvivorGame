using UnityEngine;

public class Companion : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float followDistance = 2f; // Distance from the player
    public float followSpeed = 5f; // Speed of following
    public float moveInterval = 2f; // Time interval to change direction
    public float moveRadius = 3f; // Radius within which the companion moves

    private Vector3 targetPosition;
    private Animator animator;
    private float moveTimer;

    void Start()
    {
        if (player != null)
        {
            ChooseNewTargetPosition();
        }

        animator = GetComponent<Animator>();
        moveTimer = moveInterval;
    }

    void Update()
    {
        if (player != null)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0)
            {
                ChooseNewTargetPosition();
                moveTimer = moveInterval;
            }

            MoveToTargetPosition();
        }
    }

    void ChooseNewTargetPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        targetPosition = player.position + new Vector3(randomDirection.x, randomDirection.y, 0) * moveRadius;
    }

    void MoveToTargetPosition()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
            animator.SetBool("isWalking", true); // Assuming you have an "isWalking" parameter
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
