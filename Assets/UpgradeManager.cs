using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public void UpgradeWeapon(Weapon weapon, UpgradeData upgradeData)
    {
        weapon.ApplyUpgrade(upgradeData);
    }

    public void UpgradePassiveItem(PassiveItem item)
    {
        item.ApplyUpgrade();
    }

    public void UpgradePlayer(PlayerStats playerStats, UpgradeData upgradeData)
    {
        playerStats.ApplyUpgrade(upgradeData);
    }
}
