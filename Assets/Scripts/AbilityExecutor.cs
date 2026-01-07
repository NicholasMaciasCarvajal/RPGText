using UnityEngine;

public class AbilityExecutor : MonoBehaviour
{
    public static void ExecuteAbility(
        CharacterBase caster,
        CharacterBase target,
        Ability ability)
    {
        if (!caster.SpendEnergy(ability.energyCost))
        {
            Debug.Log("No hay energía suficiente.");
            return;
        }

        Debug.Log($"{caster.name} usa {ability.abilityName} en {target.name}");

        switch (ability.abilityType)
        {
            case AbilityType.Attack:
                ExecuteAttack(caster, target, ability);
                break;

            case AbilityType.Support:
                ExecuteSupport(caster, target, ability);
                break;

            case AbilityType.Buff:
            case AbilityType.Debuff:
                ApplyStatusEffects(target, ability);
                break;
        }
    }

    private static void ExecuteAttack(
        CharacterBase caster,
        CharacterBase target,
        Ability ability)
    {
        int damage = ability.basePower + caster.attack;
        target.TakeDamage(damage);
        ApplyStatusEffects(target, ability);
    }

    private static void ExecuteSupport(
        CharacterBase caster,
        CharacterBase target,
        Ability ability)
    {
        target.Heal(ability.basePower);
        ApplyStatusEffects(target, ability);
    }

    private static void ApplyStatusEffects(
        CharacterBase target,
        Ability ability)
    {
        foreach (var effect in ability.statusEffects)
        {
            effect.ApplyEffect(target);
            Debug.Log($"{target.name} recibe efecto {effect.effectName}");
        }
    }
}
