using System.Collections.Generic;
using UnityEngine;

public class PrestigeUpgradeManager : MonoBehaviour
{
    public PrestigeUpgradeData[] prestigeUpgrades;
    private PlayerData playerData;

    private void Start()
    {
        LoadPrestigeData();  // Load player data (coins and unlocked upgrades)
    }

    public void PurchaseUpgrade(PrestigeUpgradeData upgrade)
    {
        if (!upgrade.isUnlocked && playerData.prestigeCoins >= upgrade.cost)
        {
            playerData.prestigeCoins -= upgrade.cost;
            upgrade.isUnlocked = true;
            playerData.unlockedUpgrades.Add(upgrade.upgradeName);  // Add to unlocked upgrades

            ApplyPrestigeUpgrade(upgrade);

            // Save the updated player data
            SaveManager.SavePlayerData(playerData);
        }
    }

    // Apply the permanent prestige upgrade effects to the player's stats
    private void ApplyPrestigeUpgrade(PrestigeUpgradeData upgrade)
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.maxHealth += upgrade.permanentHealthIncrease;
            playerStats.damage += upgrade.permanentDamageIncrease;
            playerStats.moveSpeed += upgrade.permanentMoveSpeedIncrease;
        }
    }

    // Load prestige data from JSON file and apply unlocked upgrades
    private void LoadPrestigeData()
    {
        playerData = SaveManager.LoadPlayerData();  // Load player data (coins and upgrades)

        // Apply previously unlocked upgrades
        foreach (var upgrade in prestigeUpgrades)
        {
            if (playerData.unlockedUpgrades.Contains(upgrade.upgradeName))
            {
                upgrade.isUnlocked = true;
                ApplyPrestigeUpgrade(upgrade);
            }
        }
    }

    // Method to add coins (optional, based on how you earn coins in your game)
    public void AddCoins(int amount)
    {
        playerData.prestigeCoins += amount;
        SaveManager.SavePlayerData(playerData);  // Save updated coin balance
    }

    // Method to get the current coin balance
    public int GetTotalCoins()
    {
        return playerData.prestigeCoins;
    }
}
