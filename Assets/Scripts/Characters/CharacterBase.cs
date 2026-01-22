using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public abstract class CharacterBase : NetworkBehaviour
{
    [Header("Base Stats")]
    public int maxHealth = 100;
    public int maxEnergy = 50;
    public int attack = 10;
    public int defense = 5;

    [Header("Current Stats")]
    public int currentHealth;
    public int currentEnergy;

    [Header("Status")]
    public bool isAlive = true;

    [Header("Status Effects")]
    public List<ActiveStatusEffect> activeStatusEffects = new List<ActiveStatusEffect>();


    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
    }

    #region Combat Methods

    public virtual void TakeTurn()
    {
        if (!isAlive)
        {
            Debug.Log($"{name} no puede actuar porque está muerto.");
            return;
        }
    }

    public virtual void TakeDamage(int amount)
    {
        if (!IsServer) return;
        if (!isAlive) return;

        int finalDamage = Mathf.Max(amount - defense, 1);
        currentHealth -= finalDamage;

        OnHealthChangedClientRpc(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual bool SpendEnergy(int amount)
    {
        if (currentEnergy < amount)
            return false;

        currentEnergy -= amount;
        return true;
    }

    public virtual void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    protected virtual void Die()
    {
        isAlive = false;
        Debug.Log($"{name} ha muerto.");
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        if (!IsServer) return;

        foreach (var active in activeStatusEffects)
        {
            if (active.baseEffect == effect)
            {
                // refrescar duración en vez de ignorar
                active.Refresh();
                return;
            }
        }

        var newEffect = new ActiveStatusEffect(effect, this);
        activeStatusEffects.Add(newEffect);

        Debug.Log($"{name} recibe efecto {effect.effectName}");
    }


    public void RemoveStatusEffect(ActiveStatusEffect effect)
    {
        activeStatusEffects.Remove(effect);
    }

    public void OnTurnStart()
    {
        foreach (var effect in new List<ActiveStatusEffect>(activeStatusEffects))
        {
            effect.OnTurnStart();
        }
    }

    public void OnTurnEnd()
    {
        foreach (var effect in new List<ActiveStatusEffect>(activeStatusEffects))
        {
            effect.OnTurnEnd();
        }
    }

    [ClientRpc]
    private void OnHealthChangedClientRpc(int newHealth)
    {
        currentHealth = newHealth;
    }

    #endregion
}
