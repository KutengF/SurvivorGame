using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    public float damage = 10f;
    public Projectile.TargetType targetType;
    public string enemyTag = "Enemy";
    public float detectionRange = 20f;
    public int multiShot = 1; // Number of additional projectiles
    public float angleOffset = 15f; // Angle offset for each additional projectile
    public int piercingCount = 0; // Default piercing count
    public bool enableHoming = false; // Homing property
    public float accuracy = 1f; // Accuracy variable, where 1 is perfect accuracy and lower values increase inaccuracy

    public enum AttackType
    {
        TargetEnemies,
        FixedDirections
    }

    public AttackType attackType; // Choose attack type in inspector

    private float nextFireTime;
    private ObjectPool objectPool;
    private string projectileTag;
    private UserController userController;

    void Start()
    {
        nextFireTime = Time.time;
        objectPool = GameObject.FindObjectOfType<ObjectPool>();
        userController = FindObjectOfType<UserController>();
        projectileTag = projectilePrefab.name;
    }

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            if (attackType == AttackType.TargetEnemies && FindNearestEnemy() != null)
            {
                Fire();
                nextFireTime = Time.time + 1f / fireRate;
            }
            else if (attackType == AttackType.FixedDirections)
            {
                FireInFixedDirections();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }

        if (attackType == AttackType.TargetEnemies)
        {
            FaceNearestEnemy();
        }
    }

    void Fire()
    {
        float baseAngle = transform.rotation.eulerAngles.z;
        GameObject target = FindNearestEnemy();
        int totalProjectiles = multiShot + 1;
        float totalAngleOffset = angleOffset * (totalProjectiles - 1);
        float startAngle = baseAngle - totalAngleOffset / 2;

        for (int i = 0; i < totalProjectiles; i++)
        {
            float angle = startAngle + i * angleOffset;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject projectile = objectPool.GetObject(projectileTag);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = rotation;

            if (target != null)
            {
                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
                directionToTarget = ApplyAccuracy(directionToTarget);
                projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);
            }

            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null)
            {
                projScript.damage = damage;
                projScript.targetType = targetType;
                projScript.SetDirection(projectile.transform.up);
                projScript.maxPenetrationCount = piercingCount; // Set the piercing count
                projScript.ResetPenetrationCount(); // Reset the penetration count when fired
                projScript.homing = enableHoming; // Set the homing property

                if (enableHoming && target != null)
                {
                    projScript.SetTarget(target.transform);
                }
            }
        }
    }

    void FireInFixedDirections()
    {
        // Define 8 directions (N, NE, E, SE, S, SW, W, NW)
        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            new Vector2(1, 1).normalized,
            Vector2.right,
            new Vector2(1, -1).normalized,
            Vector2.down,
            new Vector2(-1, -1).normalized,
            Vector2.left,
            new Vector2(-1, 1).normalized
        };

        // Get the player's facing direction
        Vector2 playerDirection = userController.GetFacingDirection().normalized;
        Vector2 selectedDirection = Vector2.up;
        float maxDot = -Mathf.Infinity;

        foreach (var direction in directions)
        {
            float dot = Vector2.Dot(playerDirection, direction);
            if (dot > maxDot)
            {
                maxDot = dot;
                selectedDirection = direction;
            }
        }

        // Fire projectiles in the selected direction with multiShot spread
        for (int i = 0; i <= multiShot; i++)
        {
            float angle = Mathf.Atan2(selectedDirection.y, selectedDirection.x) * Mathf.Rad2Deg + (i - multiShot / 2f) * angleOffset;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject projectile = objectPool.GetObject(projectileTag);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = rotation;

            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null)
            {
                projScript.damage = damage;
                projScript.targetType = targetType;
                projScript.SetDirection(projectile.transform.up);
                projScript.maxPenetrationCount = piercingCount; // Set the piercing count
                projScript.ResetPenetrationCount(); // Reset the penetration count when fired
                projScript.homing = enableHoming; // Set the homing property

                if (enableHoming)
                {
                    GameObject target = FindNearestEnemy();
                    if (target != null)
                    {
                        projScript.SetTarget(target.transform);
                    }
                }
            }
        }
    }

    void FaceNearestEnemy()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector2 direction = nearestEnemy.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= detectionRange)
            {
                nearestEnemy = enemy;
                minDistance = distance;
            }
        }

        return nearestEnemy;
    }

    public void ApplyUpgrade(UpgradeData upgradeData)
    {
        damage += upgradeData.damageIncrease;
        multiShot += upgradeData.multishotIncrease;
        fireRate += upgradeData.fireRateIncrease;
        piercingCount += upgradeData.piercingCountIncrease;
        enableHoming = upgradeData.enableHoming;
    }

    // New method to increase multishot
    public void IncreaseMultishot(int amount)
    {
        multiShot += amount;
    }

    // New method to enable or disable homing
    public void SetHoming(bool state)
    {
        enableHoming = state;
    }

    private Vector3 ApplyAccuracy(Vector3 direction)
    {
        float inaccuracyFactor = 1f - accuracy;
        float randomAngle = Random.Range(-inaccuracyFactor, inaccuracyFactor) * 10f; // Adjust the multiplier for the desired inaccuracy range
        return Quaternion.Euler(0, 0, randomAngle) * direction;
    }
}
