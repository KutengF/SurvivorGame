using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrestigeUpgradeData", menuName = "Upgrades/PrestigeUpgradeData", order = 2)]
public class PrestigeUpgradeData : ScriptableObject
{
    // Define your permanent upgrade variables
    public string upgradeName;
    public int cost;
    public bool isUnlocked;
    public float permanentHealthIncrease;
    public float permanentDamageIncrease;
    public float permanentMoveSpeedIncrease;
    // Add more permanent upgrades as needed
}
