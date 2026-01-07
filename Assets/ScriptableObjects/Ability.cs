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

    [Header("Cost")]
    public int energyCost;

    [Header("Type")]
    public AbilityType abilityType;

    [Header("Power")]
    public int basePower;

    [Header("Status Effects")]
    public List<StatusEffect> statusEffects;

    [Header("Targeting")]
    public bool targetSelf;
}
