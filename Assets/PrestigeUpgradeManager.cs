using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrestigeUpgradeManager : MonoBehaviour
{
    public PrestigeUpgradeData[] prestigeUpgrades;
    private int totalCoins;

    // Initialize or load the player's prestige coins and upgrades (from PlayerPrefs)
    private void Start()
    {
        LoadPrestigeData();  // Load unlocked upgrades when the game starts
        totalCoins = PlayerPrefs.GetInt("PrestigeCoins", 0);  // Load saved coins
    }

    // Method to purchase an upgrade
    public void PurchaseUpgrade(PrestigeUpgradeData upgrade)
    {
        if (!upgrade.isUnlocked && totalCoins >= upgrade.cost)
        {
            totalCoins -= upgrade.cost;
            upgrade.isUnlocked = true;

            ApplyPrestigeUpgrade(upgrade);

            // Save the updated data
            SavePrestigeData();
            PlayerPrefs.SetInt("PrestigeCoins", totalCoins);  // Save the updated coin balance
            PlayerPrefs.Save();  // Commit the changes to disk
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
            // Apply more permanent upgrades as necessary
        }
    }

    // Save the unlocked status of each prestige upgrade
    public void SavePrestigeData()
    {
        foreach (var upgrade in prestigeUpgrades)
        {
            // Save whether each upgrade has been unlocked
            PlayerPrefs.SetInt(upgrade.upgradeName + "_Unlocked", upgrade.isUnlocked ? 1 : 0);
        }

        PlayerPrefs.Save();  // Commit the changes to disk
    }

    // Load the unlocked status of each prestige upgrade
    public void LoadPrestigeData()
    {
        foreach (var upgrade in prestigeUpgrades)
        {
            // Load whether each upgrade has been unlocked
            upgrade.isUnlocked = PlayerPrefs.GetInt(upgrade.upgradeName + "_Unlocked", 0) == 1;
        }
    }

    // Method to add coins (optional, based on how you earn coins in your game)
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        PlayerPrefs.SetInt("PrestigeCoins", totalCoins);  // Save updated coin balance
        PlayerPrefs.Save();  // Commit the changes to disk
    }

    // Method to get the current coin balance
    public int GetTotalCoins()
    {
        return totalCoins;
    }
}
