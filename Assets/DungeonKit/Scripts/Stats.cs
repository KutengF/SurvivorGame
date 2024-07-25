using UnityEngine;

[System.Serializable]
public class Stats
{
    public int health;
    public float moveSpeed;
    public int defense;
    public float attackSpeed;
    public int damage;
    public float critChance;
    public float critDamage;
    public int mana; // For players
    public float manaRegen; // For players

    public void CopyFrom(Stats other)
    {
        health = other.health;
        moveSpeed = other.moveSpeed;
        defense = other.defense;
        attackSpeed = other.attackSpeed;
        damage = other.damage;
        critChance = other.critChance;
        critDamage = other.critDamage;
        mana = other.mana;
        manaRegen = other.manaRegen;
    }
}
