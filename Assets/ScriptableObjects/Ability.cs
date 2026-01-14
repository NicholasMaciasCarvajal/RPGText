using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    Attack,
    Support,
    Buff,
    Debuff
}

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    [Header("Info")]
    public string abilityName;
    [TextArea(3, 5)]
    public string description;

    [Header("Chances")]
    public float hitChance;

    [Header("Cost")]
    public int energyCost;

    [Header("Type")]
    public AbilityType abilityType;

    [Header("Power")]
    public int basePower;

    [Header("Status Effects")]
    public StatusEffect statusEffect;

    [Header("Targeting")]
    public bool targetSelf;

    public int RollDamage()
    {
        int damage;

        damage = basePower + statusEffect.value / 2;

        return damage;
    }
}
