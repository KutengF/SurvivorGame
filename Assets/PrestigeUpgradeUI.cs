using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrestigeUpgradeUI : MonoBehaviour
{
    public PrestigeUpgradeManager prestigeManager;

    public void OnUpgradeButtonClick(PrestigeUpgradeData upgrade)
    {
        prestigeManager.PurchaseUpgrade(upgrade);
    }
}
