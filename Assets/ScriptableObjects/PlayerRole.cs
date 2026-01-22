using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerRole", menuName = "Scriptable Objects/PlayerRole")]
public class PlayerRole : ScriptableObject
{
    [Header("Info básica")]
    public string roleName;
    [TextArea]
    public string description;

    [Header("Stats base")]
    public int baseMaxHp = 100;
    public int baseMaxEnergy = 50;
    public int baseAttack = 10;
    public int baseDefense = 5;
    public int baseSpeed = 5;

    [Header("Crecimiento por nivel")]
    public int hpPerLevel = 10;
    public int energyPerLevel = 5;
    public int attackPerLevel = 2;
    public int defensePerLevel = 1;
    public int speedPerLevel = 1;

    [Header("Habilidades iniciales")]
    public List<Ability> startingAbilities;
}
