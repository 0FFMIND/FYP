// Assets/Scripts/Inventory/InventoryModel.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    [Serializable]
    // 背包纯数据模型
    public class InventoryModel
    {
        public int Capacity { get; private set; }

        // 背包格子列表
        public List<ItemStack> Slots { get; private set; }

        public InventoryModel(int capacity)
        {
            // 至少 1 格
            Capacity = Math.Max(1, capacity);
            // 预分配容量
            Slots = new List<ItemStack>(Capacity);
            // 先填充空堆
            for (int i = 0; i < Capacity; i++)
            {
                Slots.Add(new ItemStack());
            }
        }

        public bool TryAdd(Item item, int amount)
        {
            if (item == null || amount <= 0)
            {
                return false;
            }

            // 先找是否存在同类堆（只允许存在一个）
            for (int i = 0; i < Slots.Count; i++)
            {
                var s = Slots[i];
                if (s.item == item)
                {
                    int space = item.maxStack - s.count; // 该堆剩余空间
                    if (space <= 0)
                        return false; // 堆已满，按规则失败
                    int put = Mathf.Min(space, amount); // 实际能放的数量
                    if (put <= 0)
                        return false; // 理论上不会发生，防御
                    s.count += put; // 只往这一堆加
                    Slots[i] = s; // 结构体回写
                    return true; // 只要加了一个就算成功
                }
            }

            // 没有同类堆：找第一个空槽开新堆（只开一个）
            for (int i = 0; i < Slots.Count; i++)
            {
                var s = Slots[i];
                if (s.IsEmpty)
                {
                    int put = Mathf.Min(item.maxStack, amount); // 新堆最多放 maxStack
                    if (put <= 0)
                        return false;
                    s.item = item;
                    s.count = put;
                    Slots[i] = s;
                    return true;
                }
            }

            // 没空槽可开新堆
            return false;
        }

        // 查询某个 Item 的总数量
        public int GetCount(Item item)
        {
            if (item == null)
            {
                return 0;
            }
            int total = 0;
            // 遍历所有槽位
            for (int i = 0; i < Slots.Count; i++)
            {
                var s = Slots[i];
                // 若找到该槽位
                if (s.item == item)
                    total += Mathf.Max(0, s.count);
            }
            // 返回累计结果
            return total;
        }

        public bool TryConsume(Item item, int amount = 1)
        {
            if (item == null || amount <= 0)
            {
                return false;
            }
            // 遍历所有背包槽位
            for (int i = 0; i < Slots.Count; i++)
            {
                var s = Slots[i];
                // 若该槽位是目标物品，且该堆数量足够一次扣完
                if (s.item == item && s.count >= amount)
                {
                    s.count -= amount;
                    // 扣到 0：用“空堆”替换，清掉 item 引用与数量
                    if (s.count == 0)
                    {
                        s = new ItemStack();
                    }
                    Slots[i] = s;
                    // 全部校验和扣减完成，返回成功
                    return true;
                }
            }
            // 没有堆单独满足
            return false;
        }
    }
}
