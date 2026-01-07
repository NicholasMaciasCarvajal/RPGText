using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log($"Item añadido: {item.itemName}");
    }

    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log($"Item eliminado: {item.itemName}");
        }
    }

    public void UseItem(Item item, PlayerCharacter player)
    {
        if (!items.Contains(item))
        {
            Debug.Log("No tienes este ítem");
            return;
        }

        if (item.isConsumable)
        {
            player.Heal(item.consumableHeal);
            RemoveItem(item);
            Debug.Log($"{player.name} usa {item.itemName} y recupera {item.consumableHeal} de salud.");
        }
        else
        {
            Debug.Log("Este ítem no es consumible, debe equiparse.");
        }
    }
}
