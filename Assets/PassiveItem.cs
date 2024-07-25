using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    public int currentLevel = 1;
    public int maxLevel = 10; // Expandable max level
    public UpgradeData[] upgradeData; // Array to store upgrade data for each level

    // Method to apply an upgrade
    public void ApplyUpgrade()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
        }
    }

    // Method to get the current upgrade data
    public UpgradeData GetCurrentUpgradeData()
    {
        if (currentLevel <= upgradeData.Length)
        {
            return upgradeData[currentLevel - 1];
        }
        return null;
    }

    public void ApplyEffectsToPlayer(PlayerStats playerStats)
    {
        UpgradeData data = GetCurrentUpgradeData();
        if (data != null)
        {
            playerStats.ApplyUpgrade(data);
        }
    }
}
