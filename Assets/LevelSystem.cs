using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] private Stats defaultPlayerStats;
    [SerializeField] private Stats defaultEnemyStats;
    [SerializeField] private int experienceToNextLevel = 100;

    public int playerLevel { get; private set; } = 1;
    public int enemyLevel { get; private set; } = 1;
    private int experience;

    private Stats currentPlayerStats;
    private Stats currentEnemyStats;

    void Start()
    {
        currentPlayerStats = new Stats();
        currentEnemyStats = new Stats();

        InitializeStats();
    }

    void InitializeStats()
    {
        currentPlayerStats.CopyFrom(defaultPlayerStats);
        currentEnemyStats.CopyFrom(defaultEnemyStats);
    }

    public void GainExperience(int amount)
    {
        experience += amount;
        if (experience >= experienceToNextLevel)
        {
            LevelUpPlayer();
            experience -= experienceToNextLevel;
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.1f); // Increase experience required for next level
        }
    }

    void LevelUpPlayer()
    {
        playerLevel++;
        ApplyLevelScaling(currentPlayerStats, playerLevel);
    }

    void ApplyLevelScaling(Stats stats, int level)
    {
        // Apply a smooth scaling curve for each stat
        stats.health = Mathf.RoundToInt(defaultPlayerStats.health * Mathf.Pow(1.1f, level));
        stats.moveSpeed = defaultPlayerStats.moveSpeed * Mathf.Pow(1.05f, level);
        stats.defense = Mathf.RoundToInt(defaultPlayerStats.defense * Mathf.Pow(1.05f, level));
        stats.attackSpeed = defaultPlayerStats.attackSpeed * Mathf.Pow(1.02f, level);
        stats.damage = Mathf.RoundToInt(defaultPlayerStats.damage * Mathf.Pow(1.1f, level));
        stats.critChance = defaultPlayerStats.critChance * Mathf.Pow(1.01f, level);
        stats.critDamage = defaultPlayerStats.critDamage * Mathf.Pow(1.03f, level);
        stats.mana = Mathf.RoundToInt(defaultPlayerStats.mana * Mathf.Pow(1.05f, level));
        stats.manaRegen = defaultPlayerStats.manaRegen * Mathf.Pow(1.02f, level);
    }

    public Stats GetPlayerStats()
    {
        return currentPlayerStats;
    }

    public Stats GetEnemyStats()
    {
        return currentEnemyStats;
    }
}
