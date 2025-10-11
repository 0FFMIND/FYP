using System;
using UnityEngine;

public enum ItemType { Consumable }

[CreateAssetMenu(menuName = "Item", fileName = "NewItem")]
public class Item : ScriptableObject
{
    [Header("Identity")]
    public string id;
    [TextArea] public string description;
    public ItemType type = ItemType.Consumable;
}

[Serializable]
public struct ItemStack
{
    public Item item;
    public int count;
    public bool IsEmpty => item == null || count <= 0;
}
