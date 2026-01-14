using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

    // HP sincronizado
    public NetworkVariable<int> netHealth = new NetworkVariable<int>();

    protected override void Awake()
    {
        base.Awake();

        ApplyRoleData();

        if (equipmentManager != null)
            equipmentManager.player = this;
    }

    private void Start()
    {
        if (IsServer)
            netHealth.Value = currentHealth;
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
        GameManager.Instance.turnManager.EndTurnServer();
    }

    // ------------------ COMBATE ------------------

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    // [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int amount)
    {
        if (!isAlive) return;

        currentHealth -= amount;
        netHealth.Value = currentHealth;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isAlive = false;
            Debug.Log($"{name} ha muerto");
        }
    }
}
