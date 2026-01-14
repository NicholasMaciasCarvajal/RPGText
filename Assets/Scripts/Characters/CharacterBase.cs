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
        int finalDamage = Mathf.Max(amount - defense, 1);
        currentHealth -= finalDamage;

        Debug.Log($"{name} recibe {finalDamage} de daño.");

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

    #endregion
}
