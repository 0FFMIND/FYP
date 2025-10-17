using System.Collections.Generic;
using MVC;
using UnityEngine;
using Utils;

namespace Manager
{
    public class InventoryMgr : SingletonMB<InventoryMgr>
    {
        public InventoryModel Model { get; private set; } // 当前玩家背包数据模型
        private Dictionary<string, Item> _cache;

        private void OnEnable()
        {
            EventBus.Subscribe<ESettingsChanged>(SetInventory);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ESettingsChanged>(SetInventory);
        }

        // 响应设置变更事件：从 Settings 恢复背包
        private void SetInventory(ESettingsChanged e)
        {
            var save = e.Settings.inventory;
            // 若无存档或容量非法，直接返回
            if (save == null || save.capacity <= 0)
            {
                return;
            }

            Model = new InventoryModel(save.capacity);

            // 用适配器把存档写回运行时模型
            InventorySaveAdapter.FromSave(Model, save, ResolveById);
        }

        // 最小解析器：从 Resources/Items 预热到字典，用 id 找 Item
        private Item ResolveById(string id)
        {
            // 空 id 直接返回 null（表示空格）
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            // 首次使用时构建缓存字典
            if (_cache == null)
            {
                _cache = new Dictionary<string, Item>();
                // 从 Resources/Items 加载所有 Item 资产
                foreach (var asset in Resources.LoadAll<Item>("Item"))
                {
                    if (asset && !string.IsNullOrEmpty(asset.id))
                    {
                        _cache[asset.id] = asset;
                    }
                }
            }
            return _cache.TryGetValue(id, out var it) ? it : null;
        }

        // —— 添加物品（仅叠加到已有堆，不新开空位；溢出丢弃）——
        private bool Add(Item item, int amount)
        {
            if (Model == null || item == null || amount <= 0)
            {
                return false;
            }

            bool ok = Model.TryAdd(item, amount);
            if (ok)
            {
                FlushInventorySnapshot();
            }
            return ok;
        }

        // 按 id 添加（便于脚本/存档等调用）
        public bool AddById(string itemId, int amount)
        {
            var item = ResolveById(itemId);
            return Add(item, amount);
        }

        // 按 Item 查询数量
        public int GetCount(Item item)
        {
            if (Model == null || item == null)
            {
                return 0;
            }
            return Model.GetCount(item);
        }

        // 按 id 查询数量
        public int GetCountById(string itemId)
        {
            if (Model == null)
            {
                return 0;
            }
            var item = ResolveById(itemId);
            return Model.GetCount(item);
        }

        // 按 Item 消耗（仅当单一堆可一次性扣完才成功；成功会写回存档）
        public bool TryConsume(Item item, int amount = 1)
        {
            if (Model == null)
            {
                return false;
            }
            bool ok = Model.TryConsume(item, amount);
            if (ok)
            {
                FlushInventorySnapshot();
            }
            return ok;
        }

        // 按 id 消耗（成功会写回存档）
        public bool TryConsumeById(string itemId, int amount = 1)
        {
            if (Model == null)
            {
                return false;
            }
            var item = ResolveById(itemId);
            bool ok = Model.TryConsume(item, amount);
            if (ok)
            {
                FlushInventorySnapshot();
            }
            return ok;
        }

        // —— 将当前背包快照写回 SettingsMgr（是否立刻落盘由 saveNow 决定）——
        private void FlushInventorySnapshot()
        {
            var snap = InventorySaveAdapter.ToSave(Model);
            SettingsMgr.Instance.SetInventorySnapshot(snap, true);
        }
    }
}
