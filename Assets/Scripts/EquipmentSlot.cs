using UnityEngine;

[System.Serializable]
public class EquipmentSlot
{
    public string slotName; // Ej: Slot1, Slot2, etc.
    public Item equippedItem;

    public void Equip(Item item)
    {
        equippedItem = item;
    }

    public void Unequip()
    {
        equippedItem = null;
    }

    public bool IsEmpty()
    {
        return equippedItem == null;
    }
}
