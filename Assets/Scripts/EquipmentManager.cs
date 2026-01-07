using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public PlayerCharacter player;
    public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

    private void Awake()
    {
        // Crear 5 slots automáticamente si no hay
        if (equipmentSlots.Count == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                equipmentSlots.Add(new EquipmentSlot { slotName = $"Slot{i + 1}" });
            }
        }
    }

    public void EquipItem(Item item, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipmentSlots.Count)
        {
            Debug.Log("Slot inválido");
            return;
        }

        equipmentSlots[slotIndex].Equip(item);
        RecalculateStats();
        Debug.Log($"{player.name} equipa {item.itemName} en {equipmentSlots[slotIndex].slotName}");
    }

    public void UnequipItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipmentSlots.Count) return;

        Item item = equipmentSlots[slotIndex].equippedItem;
        equipmentSlots[slotIndex].Unequip();
        RecalculateStats();
        Debug.Log($"{player.name} desequipa {item?.itemName}");
    }

    public void RecalculateStats()
    {
        int totalHealth = player.roleData.bonusHealth;
        int totalEnergy = player.roleData.bonusEnergy;
        int totalAttack = player.roleData.bonusAttack;
        int totalDefense = player.roleData.bonusDefense;

        foreach (var slot in equipmentSlots)
        {
            if (slot.equippedItem == null) continue;

            totalHealth += slot.equippedItem.bonusHealth;
            totalEnergy += slot.equippedItem.bonusEnergy;
            totalAttack += slot.equippedItem.bonusAttack;
            totalDefense += slot.equippedItem.bonusDefense;
        }

        player.maxHealth = totalHealth;
        player.maxEnergy = totalEnergy;
        player.attack = totalAttack;
        player.defense = totalDefense;

        // Asegurarse de que los stats actuales no excedan el máximo
        player.currentHealth = Mathf.Min(player.currentHealth, player.maxHealth);
        player.currentEnergy = Mathf.Min(player.currentEnergy, player.maxEnergy);
    }
}
