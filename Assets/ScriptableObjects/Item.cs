using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Accessory,
    Consumable
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea(2, 4)]
    public string description;

    [Header("Type")]
    public ItemType itemType;

    [Header("Stats Modifiers")]
    public int bonusHealth;
    public int bonusEnergy;
    public int bonusAttack;
    public int bonusDefense;

    [Header("Usable?")]
    public bool isConsumable = false;
    public int consumableHeal = 0; // solo para consumibles
}
