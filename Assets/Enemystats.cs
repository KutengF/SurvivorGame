using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    public int enemyLevel = 1;
    public float baseHealth = 50f;
    public float baseMoveSpeed = 3f;
    public float baseDefense = 5f;
    public float baseDamage = 10f;
    public float baseExperienceGiven = 20f;
    public float knockbackForce = 5f; // Knockback force, adjustable in inspector

    public float health;
    public float moveSpeed;
    public float defense;
    public float damage;
    public float experienceGiven;
    private EnemyPool enemyPool;

    public SpriteRenderer spriteRenderer; // Assign in inspector
    private Rigidbody2D rb;
    private Transform playerTransform;

    void Start()
    {
        enemyPool = GameObject.FindObjectOfType<EnemyPool>();
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateStatsBasedOnLevel();
    }

    public void UpdateStatsBasedOnLevel()
    {
        health = baseHealth * Mathf.Pow(1.1f, enemyLevel - 1); // Example scaling formula
        moveSpeed = baseMoveSpeed * Mathf.Pow(1.05f, enemyLevel - 1);
        defense = baseDefense * Mathf.Pow(1.1f, enemyLevel - 1);
        damage = baseDamage * Mathf.Pow(1.1f, enemyLevel - 1);
        experienceGiven = baseExperienceGiven * Mathf.Pow(1.2f, enemyLevel - 1);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashWhite());
            ApplyKnockback();
        }
    }

    private void ApplyKnockback()
    {
        if (playerTransform != null)
        {
            Vector2 knockbackDirection = (transform.position - playerTransform.position).normalized;
            Vector2 knockbackPosition = (Vector2)transform.position + (knockbackDirection * knockbackForce);

            // Manually move the enemy to simulate knockback
            rb.MovePosition(knockbackPosition);
        }
    }

    private void Die()
    {
        // Notify player of experience gain
        PlayerStats playerStats = GameObject.FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.GainExperience(experienceGiven);
        }

       
        // Return the enemy to the pool instead of destroying it
        enemyPool.ReturnEnemy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DamagePlayer(collision.GetComponent<PlayerStats>());
        }
    }

    private void DamagePlayer(PlayerStats playerStats)
    {
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
        }
    }

    private IEnumerator FlashWhite()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = originalColor;
    }
}
