using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Manager;
using MVC;
using UnityEngine;
using Utils;

namespace Manager
{
    public class JournalMgr : SingletonMB<JournalMgr>
    {
        // 内存中的日记条目列表（顺序即展示顺序）
        public JournalModel Model { get; private set; } = new JournalModel();

        private string jsonRelativePath = "JournalData/journal.json";

        private void Awake()
        {
            var path = Path.Combine(Application.streamingAssetsPath, jsonRelativePath);
            var json = File.ReadAllText(path);

            // 用你已有的 JsonHelper.FromJsonArray<JournalItemDTO> 解析并初始化模型
            Model.LoadFromJsonArray(json, JsonHelper.FromJsonArray<JournalItemDTO>);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ESettingsChanged>(SetJournal);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ESettingsChanged>(SetJournal);
        }

        // 响应 Settings 变更：从 Settings 恢复 Journal
        private void SetJournal(ESettingsChanged e)
        {
            if (Model == null) Model = new JournalModel();

            var save = e.Settings?.journalData;
            if (save == null)
            {
                // 没有存档则不覆盖
                return;
            }

            // 将存档状态应用到运行时模型
            JournalSaveAdapter.ApplyToModel(Model, save);
        }

        // —— 对外：切换某条日记的状态 —— 
        // 需求：传入 key 和目标 status；若目标为 Active，则补写 createdAt（UTC“首次揭示时间”）。
        // 返回：发生实际变更则 true；未找到或无变化则 false。
        public bool TrySetStatus(string key, JournalStatus targetStatus)
        {
            if (Model == null || string.IsNullOrEmpty(key))
                return false;

            var it = Model.Find(key);
            if (it == null)
                return false;

            // 切换状态
            it.status = targetStatus;

            // 目标为 Active：补写首次创建时间
            if (targetStatus == JournalStatus.Active)
            {
                it.createdAt = DateTime.Now;
            }

            FlushJournalSnapshot();
            return true;
        }

        // 向 SettingsMgr 写回当前日记快照（是否立即落盘由 SettingsMgr 决定）
        private void FlushJournalSnapshot()
        {
            if (Model == null) return;
            var snap = JournalSaveAdapter.ToSave(Model);
            SettingsMgr.Instance.SetJournalSnapshot(snap, true);
        }
    }
}
