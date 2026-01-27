using UnityEngine;

public class AbilityExecutor : MonoBehaviour
{
    public static int ExecuteAbility(
        CharacterBase caster,
        CharacterBase target,
        Ability ability)
    {
        // Seguridad: solo el servidor ejecuta lógica de combate
        if (!caster.IsServer)
        {
            Debug.LogWarning("ExecuteAbility llamado fuera del servidor.");
            return 0;
        }

        if (!caster.SpendEnergy(ability.energyCost))
        {
            Debug.Log("No hay energía suficiente.");
            return 0;
        }

        if (CombatLogManager.Instance != null)
        {
            CombatLogManager.Instance.LogServer(
                $"{caster.name} usa {ability.abilityName} en {target.name}"
            );
        }

        int damageDealt = 0;

        switch (ability.abilityType)
        {
            case AbilityType.Attack:
                damageDealt = ExecuteAttack(caster, target, ability);
                break;

            case AbilityType.Support:
                ExecuteSupport(caster, target, ability);
                break;

            case AbilityType.Buff:
            case AbilityType.Debuff:
                ApplyStatusEffects(target, ability);
                break;
        }

        return damageDealt;
    }

    private static int ExecuteAttack(
        CharacterBase caster,
        CharacterBase target,
        Ability ability)
    {
        int damage = ability.basePower + caster.attack;

        target.TakeDamage(damage);

        ApplyStatusEffects(target, ability);

        return damage;
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
        if (ability.statusEffect != null)
        {
            target.AddStatusEffect(ability.statusEffect);
        }
    }
}
