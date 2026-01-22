using UnityEngine;

public class ActiveStatusEffect
{
    public StatusEffect baseEffect;
    public int remainingDuration;

    private CharacterBase target;

    public ActiveStatusEffect(StatusEffect effect, CharacterBase target)
    {
        baseEffect = effect;
        remainingDuration = effect.duration;
        this.target = target;

        Apply();
    }

    private void Apply()
    {
        if (!target.isAlive) return;

        switch (baseEffect.effectType)
        {
            case StatusEffectType.BuffAttack:
                target.attack += baseEffect.value;
                break;

            case StatusEffectType.BuffDefense:
                target.defense += baseEffect.value;
                break;

            case StatusEffectType.DebuffAttack:
                target.attack -= baseEffect.value;
                break;

            case StatusEffectType.DebuffDefense:
                target.defense -= baseEffect.value;
                break;
        }
    }

    public void OnTurnStart()
    {
        if (!target.isAlive) return;

        // DOT se ejecuta cada turno
        if (baseEffect.effectType == StatusEffectType.DamageOverTime)
        {
            target.TakeDamage(baseEffect.value);
            Debug.Log($"{target.name} recibe {baseEffect.value} de daño por {baseEffect.effectName}");
        }
    }

    public void OnTurnEnd()
    {
        remainingDuration--;

        if (remainingDuration <= 0)
        {
            Expire();
        }
    }

    private void Expire()
    {
        // revertir buffs/debuffs
        switch (baseEffect.effectType)
        {
            case StatusEffectType.BuffAttack:
                target.attack -= baseEffect.value;
                break;

            case StatusEffectType.BuffDefense:
                target.defense -= baseEffect.value;
                break;

            case StatusEffectType.DebuffAttack:
                target.attack += baseEffect.value;
                break;

            case StatusEffectType.DebuffDefense:
                target.defense += baseEffect.value;
                break;
        }

        target.RemoveStatusEffect(this);
        Debug.Log($"{baseEffect.effectName} ha expirado en {target.name}");
    }

    // Permite refrescar duración si se reaplica
    public void Refresh()
    {
        remainingDuration = baseEffect.duration;
        Debug.Log($"{baseEffect.effectName} se refresca en {target.name}");
    }
}
