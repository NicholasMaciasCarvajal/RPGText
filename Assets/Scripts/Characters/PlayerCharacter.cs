using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : CharacterBase
{
    [Header("Role")]
    public RoleData roleData;

    [Header("Systems")]
    public Inventory inventory;
    public EquipmentManager equipmentManager;

    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();

    private bool inputEnabled = false;

    protected override void Awake()
    {
        base.Awake();

        ApplyRoleData();
        // Asignar referencia de PlayerCharacter al EquipmentManager
        if (equipmentManager != null)
            equipmentManager.player = this;
    }

    private void ApplyRoleData()
    {
        if (roleData == null) return;

        maxHealth += roleData.bonusHealth;
        maxEnergy += roleData.bonusEnergy;
        attack += roleData.bonusAttack;
        defense += roleData.bonusDefense;

        abilities.AddRange(roleData.initialAbilities);

        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
    }

    public void EnableInput(bool enable)
    {
        inputEnabled = enable;

        if (enable)
            Debug.Log($"{name} puede actuar.");
    }

    public bool CanAct()
    {
        return inputEnabled && isAlive;
    }

    public void EndTurn()
    {
        inputEnabled = false;
        GameManager.Instance.turnManager.EndTurn();
    }
}
