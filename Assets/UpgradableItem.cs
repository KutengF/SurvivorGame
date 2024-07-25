using UnityEngine;

public abstract class UpgradableItem : MonoBehaviour
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
            ApplyUpgradeEffects();
        }
    }

    // Abstract method to apply upgrade effects, to be implemented by derived classes
    protected abstract void ApplyUpgradeEffects();
}
