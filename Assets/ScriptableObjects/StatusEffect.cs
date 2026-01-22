using UnityEngine;

public enum StatusEffectType
{
    DamageOverTime,
    BuffAttack,
    BuffDefense,
    DebuffAttack,
    DebuffDefense
}

[CreateAssetMenu(fileName = "StatusEffect", menuName = "Scriptable Objects/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    [Header("Info")]
    public string effectName;
    public StatusEffectType effectType;

    [Header("Values")]
    public int value;
    public int duration; // turnos
}
