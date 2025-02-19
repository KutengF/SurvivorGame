using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    // Player stats with base values
    public float health = 100f;
    public float currentHealth;
    public float healthRegen = 1f; // Health regeneration per second
    public float moveSpeed = 5f;
    public float defense = 10f;
    public float attackSpeed = 1f;
    public float damage = 20f;
    public float critChance = 0.1f;
    public float critDamage = 1.5f;

    // Prestige-based upgrades (to be applied on game start)
    public float permanentHealthBonus = 0f;
    public float permanentDamageBonus = 0f;
    public float permanentMoveSpeedBonus = 0f;
    // Add more prestige bonuses as needed

    // Leveling system
    public float experience = 0f;
    public int level = 1;
    public int gold = 0;
    public int materials = 0;
    public HealthBar healthBar;
    public ExperienceBar experienceBar;
    public float maxExperience = 100f;

    public GameObject gameOverPanel;

    // Item slots and default items
    public int numberOfSlots = 6; // Number of item slots, adjustable in the inspector
    public Transform[] itemSlots; // Array to store item slots
    public GameObject[] defaultItemPrefabs; // Assign the default item prefabs in the inspector

    private List<PassiveItem> passiveItems = new List<PassiveItem>();
    private List<Weapon> weapons = new List<Weapon>();

    [SerializeField] GameObject UpgradePanel;

    private void Start()
    {
        // Initialize item slots
        itemSlots = new Transform[numberOfSlots];
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = new GameObject("Slot" + (i + 1));
            slot.transform.parent = this.transform;
            itemSlots[i] = slot.transform;
        }

        // Assign default items to the slots
        if (defaultItemPrefabs != null)
        {
            for (int i = 0; i < defaultItemPrefabs.Length && i < numberOfSlots; i++)
            {
                var item = Instantiate(defaultItemPrefabs[i], itemSlots[i]);
                var passiveItem = item.GetComponent<PassiveItem>();
                if (passiveItem != null)
                {
                    passiveItems.Add(passiveItem);
                }

                var weapon = item.GetComponent<Weapon>();
                if (weapon != null)
                {
                    weapons.Add(weapon);
                }
            }
        }

        // Apply permanent prestige upgrades
        LoadPermanentStats();

        // Initialize current health to max health after applying bonuses
        currentHealth = health;
        experienceBar.SetMaxExperience(maxExperience);
    }

    private void Update()
    {
        healthBar.SetMaxHealth(health);

        // Debug: Press "L" to level up
        if (Input.GetKeyDown(KeyCode.L))
        {
            ManualLevelUp();
        }

        // Regenerate health over time
        if (currentHealth < health)
        {
            currentHealth += healthRegen * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, health); // Ensure current health does not exceed max health
        }
        healthBar.SetHealth(currentHealth);

        // Apply passive item effects
        ApplyPassiveItemEffects();
    }

    // Method to load and apply permanent prestige upgrades at the start of the game
    private void LoadPermanentStats()
    {
        PrestigeUpgradeManager prestigeManager = FindObjectOfType<PrestigeUpgradeManager>();

        if (prestigeManager != null)
        {
            foreach (var upgrade in prestigeManager.prestigeUpgrades)
            {
                if (upgrade.isUnlocked)
                {
                    health += upgrade.permanentHealthIncrease;
                    damage += upgrade.permanentDamageIncrease;
                    moveSpeed += upgrade.permanentMoveSpeedIncrease;

                    // Apply more permanent upgrades as necessary
                }
            }
        }

        // Apply permanent bonuses to stats
        health += permanentHealthBonus;
        damage += permanentDamageBonus;
        moveSpeed += permanentMoveSpeedBonus;
    }

    private void ApplyPassiveItemEffects()
    {
        foreach (var item in passiveItems)
        {
            item.ApplyEffectsToPlayer(this);
        }
    }

    // Method to gain experience
    public void GainExperience(float amount)
    {
        experience += amount;
        experienceBar.SetExperience(experience);
        CheckLevelUp();
    }

    // Check if the player can level up
    private void CheckLevelUp()
    {
        while (experience >= GetExperienceForNextLevel())
        {
            experience -= GetExperienceForNextLevel();
            level++;
            LevelUp();
        }
    }

    // Calculate the experience required for the next level
    private float GetExperienceForNextLevel()
    {
        return 100 * Mathf.Pow(level, 1.5f); // Adjust this formula as needed
    }

    // Level up the player and increase stats
    private void LevelUp()
    {
        maxExperience = GetExperienceForNextLevel();
        experienceBar.SetMaxExperience(maxExperience);
        //UpgradePanel.SetActive(true); // Disable for LevelUp Panel is not available yet.
        Debug.Log($"Level Up! Current Level: {level}");
    }

    // Manual level up for debugging
    private void ManualLevelUp()
    {
        experience += GetExperienceForNextLevel();
        CheckLevelUp();
    }

    // Method to take damage
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Handle player death
    private void Die()
    {
        Debug.Log("Player has died.");
        Time.timeScale = 0f; // Pause the game
        gameOverPanel.SetActive(true); // Show game over panel
    }

    // Add similar methods for other stats as needed

    // Check if there are any free item slots
    public bool HasFreeItemSlot()
    {
        foreach (var slot in itemSlots)
        {
            if (slot.childCount == 0)
            {
                return true;
            }
        }
        return false;
    }

    // Add an item to the first available slot
    public void AddItem(GameObject itemPrefab)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.childCount == 0)
            {
                var item = Instantiate(itemPrefab, slot);
                var passiveItem = item.GetComponent<PassiveItem>();
                if (passiveItem != null)
                {
                    passiveItems.Add(passiveItem);
                }

                var weapon = item.GetComponent<Weapon>();
                if (weapon != null)
                {
                    weapons.Add(weapon);
                }
                break;
            }
        }
    }

    // Apply temporary upgrades during gameplay
    public void ApplyUpgrade(UpgradeData upgradeData)
    {
        health += health * (upgradeData.healthIncreasePercentage / 100f);
        currentHealth = health; // Update current health to new max health
        healthRegen *= (1 + upgradeData.regenIncreasePercentage / 100f);
        moveSpeed *= (1 + upgradeData.moveSpeedIncreasePercentage / 100f);
        defense *= (1 + upgradeData.defenseIncreasePercentage / 100f);
        attackSpeed *= (1 + upgradeData.attackSpeedIncreasePercentage / 100f);
        damage *= (1 + upgradeData.damageIncreasePercentage / 100f);
        critChance *= (1 + upgradeData.critChanceIncreasePercentage / 100f);
        critDamage *= (1 + upgradeData.critDamageIncreasePercentage / 100f);

        foreach (var weapon in weapons)
        {
            weapon.ApplyUpgrade(upgradeData);
        }
    }

    public float GetCurrentMoveSpeed()
    {
        return moveSpeed;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
