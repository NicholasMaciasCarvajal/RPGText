using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoleData", menuName = "Scriptable Objects/RoleData")]
public class RoleData : ScriptableObject
{
    [Header("Role Info")]
    public string roleName;
    [TextArea(3, 5)]
    public string description;

    [Header("Stat Bonuses")]
    public int bonusHealth;
    public int bonusEnergy;
    public int bonusAttack;
    public int bonusDefense;

    [Header("Abilities")]
    public List<Ability> initialAbilities;

    [Header("Passive Modifiers")]
    public List<RolePassive> passives;
}
