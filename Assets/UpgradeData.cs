using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrades/UpgradeData", order = 1)]
public class UpgradeData : ScriptableObject
{
    // Weapon upgrade variables
    public float damageIncrease;
    public int multishotIncrease;
    public float fireRateIncrease;
    public int piercingCountIncrease;
    public bool enableHoming;

    // Player stat upgrade variables
    public float healthIncreasePercentage;
    public float regenIncreasePercentage;
    public float moveSpeedIncreasePercentage;
    public float defenseIncreasePercentage;
    public float attackSpeedIncreasePercentage;
    public float damageIncreasePercentage;
    public float critChanceIncreasePercentage;
    public float critDamageIncreasePercentage;
}
