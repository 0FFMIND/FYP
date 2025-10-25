using MVC;
using System;
using System.Collections.Generic;
using UnityEngine;

// 允许被 JSON/Unity 序列化
[Serializable]
public class InventorySaveData
{
    public int capacity;
    public List<string> itemIds = new(); // 每格 item 的唯一 id（空格填 ""）
    public List<int> counts = new(); // 每格数量（空格填 0）
}

public static class InventorySaveAdapter
{
    public static InventorySaveData ToSave(InventoryModel m)
    {
        // 记录当前格子数
        var d = new InventorySaveData { capacity = m.Slots.Count };
        // 遍历每个格子
        for (int i = 0; i < m.Slots.Count; i++)
        {
            var s = m.Slots[i];
            d.itemIds.Add(s.IsEmpty ? "" : (s.item ? s.item.id : ""));
            d.counts.Add(s.IsEmpty ? 0 : s.count);
        }
        return d;
    }

    // resolver: 通过 id 还原 ScriptableObject Item（传入一个 id->Item 的解析函数）
    public static void FromSave(InventoryModel m, InventorySaveData d, Func<string, Item> resolver)
    {
        int n = Mathf.Min(m.Slots.Count, d.itemIds.Count);
        for (int i = 0; i < n; i++)
        {
            string id = d.itemIds[i];
            int c = (i < d.counts.Count) ? d.counts[i] : 0;
            if (string.IsNullOrEmpty(id) || c <= 0)
            {
                m.Slots[i] = new ItemStack();
            }
            else
            {
                // 通过ID解析到 Item
                var def = resolver(id);
                m.Slots[i] = new ItemStack { item = def, count = c };
            }
        }
    }
}
