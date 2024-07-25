using UnityEngine;
using System.Collections; // Added this directive for IEnumerator

public class Projectile : MonoBehaviour
{
    public enum TargetType
    {
        Player,
        Enemy
    }

    public enum TrajectoryType
    {
        Normal,
        Spinning,
        Spiral
    }

    public float speed = 10f;
    public float damage = 10f;
    public TargetType targetType;
    public TrajectoryType trajectoryType; // New field for trajectory type
    public int maxPenetrationCount = 0; // Default to 0, meaning no penetration
    public bool homing = false; // Homing property

    private Vector2 direction;
    private int currentPenetrationCount = 0;
    private ObjectPool objectPool;
    private Transform target;
    private Transform playerTransform;
    private float angle;
    public float radius=0.5f;

    void Start()
    {
        direction = transform.up;
        objectPool = GameObject.FindObjectOfType<ObjectPool>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        angle = 0f;
       
        StartCoroutine(ReturnToPoolAfterDelay(1f)); // Start the coroutine to return to pool after 1 second
    }

    void Update()
    {
        switch (trajectoryType)
        {
            case TrajectoryType.Normal:
                MoveNormal();
                break;
            case TrajectoryType.Spinning:
                MoveSpinning();
                break;
            case TrajectoryType.Spiral:
                MoveSpiral();
                break;
        }
    }

    private void MoveNormal()
    {
        if (homing && target != null)
        {
            Vector2 targetDirection = (target.position - transform.position).normalized;
            direction = Vector2.Lerp(direction, targetDirection, Time.deltaTime * speed).normalized;
        }

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        RotateTowardsMovementDirection(direction);
    }

    private void MoveSpinning()
    {
        angle += speed * Time.deltaTime;
        float x = playerTransform.position.x + Mathf.Cos(angle) * radius;
        float y = playerTransform.position.y + Mathf.Sin(angle) * radius;
        Vector2 newPos = new Vector2(x, y);
        Vector2 direction = (newPos - (Vector2)transform.position).normalized;
        transform.position = newPos;
        RotateTowardsMovementDirection(direction);
    }

    private void MoveSpiral()
    {
        angle += speed * Time.deltaTime;
        radius += 0.1f * Time.deltaTime; // Increase radius over time for spiral effect
        float x = playerTransform.position.x + Mathf.Cos(angle) * radius;
        float y = playerTransform.position.y + Mathf.Sin(angle) * radius;
        Vector2 newPos = new Vector2(x, y);
        Vector2 direction = (newPos - (Vector2)transform.position).normalized;
        transform.position = newPos;
        RotateTowardsMovementDirection(direction);
    }

    private void RotateTowardsMovementDirection(Vector2 movementDirection)
    {
        float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Adjust the angle as needed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetType == TargetType.Enemy && collision.CompareTag("Enemy")) ||
            (targetType == TargetType.Player && collision.CompareTag("Player")))
        {
            if (targetType == TargetType.Enemy)
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(damage);
                }
            }
            else if (targetType == TargetType.Player)
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.currentHealth -= damage;
                    Debug.Log("Player Health: " + playerStats.GetCurrentHealth());
                }
            }

            currentPenetrationCount++;
            if (currentPenetrationCount > maxPenetrationCount)
            {
                // Return the projectile to the pool
                objectPool.ReturnObject(gameObject);
            }
            else
            {
                // Switch to a random enemy if piercing count is higher than 1
                if (maxPenetrationCount > 1)
                {
                    SwitchToRandomEnemy();
                }
            }
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ResetPenetrationCount()
    {
        currentPenetrationCount = 0;
    }

    private void SwitchToRandomEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            target = enemies[Random.Range(0, enemies.Length)].transform;
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        objectPool.ReturnObject(gameObject);
    }
}
