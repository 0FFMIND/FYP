using System;
using UnityEngine;

public enum ItemType
{
    Consumable,
}

[CreateAssetMenu(menuName = "Item", fileName = "NewItem")]
public class Item : ScriptableObject
{
    [Header("Identity")]
    // 物品唯一ID（用于存档/查表）
    public string id;

    // 文本描述（多行输入）
    [TextArea]
    public string description;

    // 物品类型（默认消耗品）
    public ItemType type = ItemType.Consumable;

    // 允许最大堆积数
    public int maxStack = 99;
}

[Serializable]
// 可序列化的“物品堆栈”数据（放在背包格子里）
public struct ItemStack
{
    // 指向某个 Item 资产（物品定义）
    public Item item;

    // 当前堆栈数量
    public int count;
    public bool IsEmpty => item == null || count <= 0;
    public int SpaceLeft => (item == null) ? 0 : (item.maxStack - count);

}
