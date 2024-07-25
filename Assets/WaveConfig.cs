using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Config", menuName = "Wave Config", order = 1)]
public class WaveConfig : ScriptableObject
{
    public List<GameObject> enemyPrefabs; // List of enemy prefabs
    public List<float> enemySpawnRates; // List of spawn rates for each enemy type
    public int enemyCount; // Total number of enemies in the wave
    public float spawnRateMultiplier = 1f; // Multiplier for spawn rate
    public bool isBossWave = false;
    public GameObject bossPrefab; // Boss prefab for boss waves
    public bool isTimeBased = false;
    public float waveDuration = 30f; // Duration of the wave in seconds
    public int enemiesPerTick = 5; // Number of enemies to spawn per tick
}
