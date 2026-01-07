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

    public void ApplyEffect(CharacterBase target)
    {
        switch (effectType)
        {
            case StatusEffectType.DamageOverTime:
                target.TakeDamage(value);
                break;

            case StatusEffectType.BuffAttack:
                target.attack += value;
                break;

            case StatusEffectType.BuffDefense:
                target.defense += value;
                break;

            case StatusEffectType.DebuffAttack:
                target.attack -= value;
                break;

            case StatusEffectType.DebuffDefense:
                target.defense -= value;
                break;
        }
    }

    public void RemoveEffect(CharacterBase target)
    {
        switch (effectType)
        {
            case StatusEffectType.BuffAttack:
                target.attack -= value;
                break;

            case StatusEffectType.BuffDefense:
                target.defense -= value;
                break;

            case StatusEffectType.DebuffAttack:
                target.attack += value;
                break;

            case StatusEffectType.DebuffDefense:
                target.defense += value;
                break;
        }
    }
}
