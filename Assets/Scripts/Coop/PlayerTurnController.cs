using UnityEngine;
using System.Collections.Generic;

public class PlayerTurnController : MonoBehaviour
{
    private PlayerCharacter player;
    private Inventory inventory;
    private EquipmentManager equipment;

    [Header("Current Selection")]
    public Ability selectedAbility;
    public Item selectedItem;

    private void Awake()
    {
        player = GetComponent<PlayerCharacter>();
        inventory = player.inventory;
        equipment = player.equipmentManager;

        if (player == null)
            Debug.LogError("PlayerTurnController requiere PlayerCharacter");
    }

    private void Update()
    {
        if (!player.CanAct()) return; // Solo puede actuar si es su turno

        // **Aquí iría tu UI de selección de habilidades/items**
        // Por ejemplo:
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Selecciona habilidad 1
        {
            UseAbility(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // Usar item 1
        {
            UseItem(0);
        }
    }

    #region Acciones del jugador

    public void UseAbility(int abilityIndex)
    {
        if (abilityIndex < 0 || abilityIndex >= player.abilities.Count) return;

        selectedAbility = player.abilities[abilityIndex];
        if (!player.SpendEnergy(selectedAbility.energyCost))
        {
            Debug.Log("No hay energía suficiente para usar esta habilidad.");
            return;
        }

        // Selección simple de objetivo: enemigo vivo
        EnemyCharacter target = BattleManager.Instance.GetRandomAliveEnemy();
        if (target == null)
        {
            Debug.Log("No hay enemigos vivos.");
            return;
        }

        AbilityExecutor.ExecuteAbility(player, target, selectedAbility);

        EndTurn();
    }

    public void UseItem(int itemIndex)
    {
        if (inventory.items.Count == 0 || itemIndex >= inventory.items.Count) return;

        selectedItem = inventory.items[itemIndex];
        inventory.UseItem(selectedItem, player);

        EndTurn();
    }

    private void EndTurn()
    {
        player.EnableInput(false);
        CoopManager.Instance.EndCurrentPlayerTurn(player);
    }

    #endregion
}
