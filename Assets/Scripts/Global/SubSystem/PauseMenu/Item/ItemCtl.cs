// Assets/Scripts/Inventory/InventoryListCtl.cs
using Manager;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MVC
{
    /// <summary>
    /// 背包展示控制器：OnEnable 从 InventoryMgr 读取 Model，生成若干行。
    /// 空格跳过；左名右数；名称由内部 switch(id) 决定。
    /// </summary>
    public class ItemCtl : MonoBehaviour
    {
        [SerializeField]
        private ItemView itemView;

        [SerializeField]
        private Transform viewRoot;

        private void OnEnable()
        {
            RebuildFromInventory();
        }

        private void RebuildFromInventory()
        {
            if (viewRoot == null)
            {
                Debug.LogError("[InventoryListCtl] 请在 Inspector 绑定 viewRoot");
                return;
            }

            ClearChildren();

            var mgr = InventoryMgr.Instance;
            if (mgr == null || mgr.Model == null)
                return;

            var slots = mgr.Model.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                var s = slots[i];
                if (s.IsEmpty)
                    continue;

                string displayName = PrettyItemName(s.item ? s.item.id : null);
                var row = Instantiate(itemView, viewRoot);
                row.name = $"ItemRow_{s.item.id}_{i}"; // 可选：便于调试在层级里识别
                row.Bind(displayName, s.count); // 左边名字，右边数量
            }
        }

        private void ClearChildren()
        {
            if (viewRoot == null)
                return;
            for (int i = viewRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(viewRoot.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 你可以在这里维护 id → 展示名 的映射。
        /// 按需补充/修改；未命中时回退到 id 原文。
        /// </summary>
        private string PrettyItemName(string id) =>
            id switch
            {
                "coin" => "硬币",
                _ => id,
            };
    }
}
