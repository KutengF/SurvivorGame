using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<WaveConfig> waves; // List of wave configurations
    public Transform player; // Reference to the player transform
    public float spawnRadius = 10f; // Radius around the player where enemies can spawn
    public float minSpawnDistance = 5f; // Minimum distance from the player for enemy spawning
    public float waveInterval = 30f; // Interval between waves
    public float baseSpawnDelay = 1.0f; // Base delay between enemy spawns
    public float minimumSpawnDelay = 0.1f; // Minimum delay between enemy spawns

    private int currentWaveIndex = 0; // Index of the current wave
    private bool isSpawning = false; // Flag to control spawning
    private EnemyPool enemyPool;
    private bool gameIsOver = false;

    void Start()
    {
        enemyPool = GameObject.FindObjectOfType<EnemyPool>();
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (currentWaveIndex < waves.Count)
        {
            yield return new WaitForSeconds(waveInterval);
            isSpawning = true;
            SpawnWave(waves[currentWaveIndex]);
            yield return new WaitUntil(() => !isSpawning); // Wait until the wave is complete
            currentWaveIndex++;
        }

        gameIsOver = true;
    }

    void SpawnWave(WaveConfig waveConfig)
    {
        if (waveConfig.isBossWave)
        {
            SpawnBossWave(waveConfig);
        }
        else if (waveConfig.isTimeBased)
        {
            StartCoroutine(SpawnTimeBasedWave(waveConfig));
        }
        else
        {
            StartCoroutine(SpawnEnemiesCoroutine(waveConfig));
        }
    }

    void SpawnBossWave(WaveConfig waveConfig)
    {
        Vector2 spawnPosition;
        do
        {
            Vector2 spawnDirection = Random.insideUnitCircle.normalized; // Get a random direction
            float spawnDistance = Random.Range(minSpawnDistance, spawnRadius); // Random distance within the allowed range
            spawnPosition = (Vector2)player.position + spawnDirection * spawnDistance;
        }
        while (Vector2.Distance(spawnPosition, player.position) < minSpawnDistance);

        GameObject boss = Instantiate(waveConfig.bossPrefab, spawnPosition, Quaternion.identity);
        EnemyStats bossStats = boss.GetComponent<EnemyStats>();
        if (bossStats != null)
        {
            bossStats.enemyLevel = DetermineEnemyLevel();
            bossStats.UpdateStatsBasedOnLevel();
        }

        isSpawning = false;
    }

    IEnumerator SpawnTimeBasedWave(WaveConfig waveConfig)
    {
        float spawnDelay = Mathf.Max(minimumSpawnDelay, baseSpawnDelay / waveConfig.spawnRateMultiplier);
        float waveEndTime = Time.time + waveConfig.waveDuration;

        while (Time.time < waveEndTime)
        {
            for (int i = 0; i < waveConfig.enemiesPerTick; i++)
            {
                GameObject enemyPrefab = GetRandomEnemyBasedOnSpawnRate(waveConfig);

                Vector2 spawnPosition;
                do
                {
                    Vector2 spawnDirection = Random.insideUnitCircle.normalized; // Get a random direction
                    float spawnDistance = Random.Range(minSpawnDistance, spawnRadius); // Random distance within the allowed range
                    spawnPosition = (Vector2)player.position + spawnDirection * spawnDistance;
                }
                while (Vector2.Distance(spawnPosition, player.position) < minSpawnDistance);

                GameObject enemy = enemyPool.GetEnemy(enemyPrefab.name);
                if (enemy != null)
                {
                    enemy.transform.position = spawnPosition;
                    enemy.transform.rotation = Quaternion.identity;

                    // Determine and assign enemy level
                    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.enemyLevel = DetermineEnemyLevel();
                        enemyStats.UpdateStatsBasedOnLevel();
                    }
                }
            }
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }

    IEnumerator SpawnEnemiesCoroutine(WaveConfig waveConfig)
    {
        float spawnDelay = Mathf.Max(minimumSpawnDelay, baseSpawnDelay / waveConfig.spawnRateMultiplier);

        for (int i = 0; i < waveConfig.enemyCount;)
        {
            for (int j = 0; j < waveConfig.enemiesPerTick && i < waveConfig.enemyCount; j++, i++)
            {
                GameObject enemyPrefab = GetRandomEnemyBasedOnSpawnRate(waveConfig);

                Vector2 spawnPosition;
                do
                {
                    Vector2 spawnDirection = Random.insideUnitCircle.normalized; // Get a random direction
                    float spawnDistance = Random.Range(minSpawnDistance, spawnRadius); // Random distance within the allowed range
                    spawnPosition = (Vector2)player.position + spawnDirection * spawnDistance;
                }
                while (Vector2.Distance(spawnPosition, player.position) < minSpawnDistance);

                GameObject enemy = enemyPool.GetEnemy(enemyPrefab.name);
                if (enemy != null)
                {
                    enemy.transform.position = spawnPosition;
                    enemy.transform.rotation = Quaternion.identity;

                    // Determine and assign enemy level
                    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.enemyLevel = DetermineEnemyLevel();
                        enemyStats.UpdateStatsBasedOnLevel();
                    }
                }
            }
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }

    private GameObject GetRandomEnemyBasedOnSpawnRate(WaveConfig waveConfig)
    {
        float totalSpawnRate = 0f;
        foreach (float spawnRate in waveConfig.enemySpawnRates)
        {
            totalSpawnRate += spawnRate;
        }

        float randomValue = Random.Range(0f, totalSpawnRate);
        float cumulativeRate = 0f;
        for (int i = 0; i < waveConfig.enemySpawnRates.Count; i++)
        {
            cumulativeRate += waveConfig.enemySpawnRates[i];
            if (randomValue < cumulativeRate)
            {
                return waveConfig.enemyPrefabs[i];
            }
        }

        return waveConfig.enemyPrefabs[0]; // Fallback in case of an error
    }

    private int DetermineEnemyLevel()
    {
        int baseLevel = 1; // Minimum enemy level
        int waveFactor = currentWaveIndex / 5; // Increase level every 5 waves
        int randomVariation = Random.Range(-1, 2); // Small random variation

        return Mathf.Max(1, baseLevel + waveFactor + randomVariation); // Ensure level is at least 1
    }

    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }

    public bool IsGameOver()
    {
        return gameIsOver;
    }
}
