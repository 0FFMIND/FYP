using System;
using System.Collections.Generic;
using MVC;
using UnityEngine;
using Utils;

namespace Manager
{
    public class JournalMgr : SingletonMB<JournalMgr>
    {
        // 内存中的日记条目列表（顺序即展示顺序）
        public JournalModel Model { get; private set; } = new JournalModel();

        private void Awake()
        {
            // 解析SO并初始化模型
            Model.LoadFromSO();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ESettingsChanged>(SetJournal);
            EventBus.Subscribe<EJournalStepChanged>(OnStepChanged);
            EventBus.Subscribe<EJournalStatusChanged>(OnStatusChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ESettingsChanged>(SetJournal);
            EventBus.Unsubscribe<EJournalStepChanged>(OnStepChanged);
            EventBus.Unsubscribe<EJournalStatusChanged>(OnStatusChanged);
        }

        // 处理“某条日记的某个 Step 状态变化”的事件
        private void OnStepChanged(EJournalStepChanged e)
        {
            // 通过 key 找到对应的 JournalItem（可能为 null）
            var it = Model?.Find(e.Key);
            if (it == null)
                return;

            // 找到所有 Step 在 contents 里的下标
            int targetContentIdx = -1;
            if (it.contents != null)
            {
                var stepIndices = new List<int>();
                for (int i = 0; i < it.contents.Count; i++)
                {
                    var ln = it.contents[i];
                    if (ln?.line != null && ln.line.Kind == JournalLineKind.Step)
                        stepIndices.Add(i);
                }

                targetContentIdx = stepIndices[e.Index];

                // 找到了对应内容位置
                if (targetContentIdx != -1)
                {
                    // —— 状态未变化 → 直接返回（避免无谓刷新/激活/落盘）
                    if (it.contents[targetContentIdx].State == e.State)
                        return;
                    // 把该 Step 的可视状态更新为事件传来的状态
                    it.contents[targetContentIdx].State = e.State;
                }
                else
                {
                    Debug.LogWarning(
                        $"[JournalMgr] Step index out of range: key={e.Key}, index={e.Index}, steps={stepIndices.Count}"
                    );
                }
            }
            // 如果当前条目标记为 Hidden
            if (it.status == JournalStatus.Hidden)
            {
                // 把条目激活为 Active，但保留已存在的 Step 状态
                TrySetStatus(e.Key, JournalStatus.Active, resetSteps: false);
            }

            // 如果所有 Step 都完成，则把条目标为 Completed
            if (AllStepsDone(it) && it.status != JournalStatus.Completed)
            {
                it.status = JournalStatus.Completed;
                EventBus.Publish(new EJournalStatusChanged(e.Key, it.status));
            }

            // 保存刷新
            FlushJournalSnapshot();
        }

        private static bool AllStepsDone(JournalItem it)
        {
            if (it?.contents == null)
                return true;
            foreach (var c in it.contents)
                if (
                    c?.line != null
                    && c.line.Kind == JournalLineKind.Step
                    && c.State != StepState.Done
                )
                    return false;
            return true;
        }

        // 响应 Settings 变更：从 Settings 恢复 Journal
        private void SetJournal(ESettingsChanged e)
        {
            if (Model == null)
                Model = new JournalModel();

            var save = e.Settings?.journalData;
            if (save == null)
            {
                // 没有存档则不覆盖
                return;
            }

            // 将存档状态应用到运行时模型
            JournalSaveAdapter.ApplyToModel(Model, save);
            print(Model.ToString());
        }

        private void OnStatusChanged(EJournalStatusChanged e)
        {
            // 已是该状态则不重复写入，避免副作用（例如错误重置步骤）
            var it = Model?.Find(e.Key);
            if (it != null && it.status == e.NewStatus)
                return;
            TrySetStatus(e.Key, e.NewStatus);
        }

        // —— 对外：切换某条日记的状态 ——
        public bool TrySetStatus(string key, JournalStatus targetStatus) =>
            TrySetStatus(key, targetStatus, resetSteps: true);

        // 需求：传入 key 和目标 status；若目标为 Active，则补写 createdAt（UTC“首次揭示时间”）。
        // 返回：发生实际变更则 true；未找到或无变化则 false。
        public bool TrySetStatus(string key, JournalStatus targetStatus, bool resetSteps)
        {
            if (Model == null || string.IsNullOrEmpty(key))
                return false;

            var it = Model.Find(key);
            if (it == null)
                return false;

            // 目标状态与当前一致：直接返回 false（既不写 createdAt，也不重置步骤）
            if (it.status == targetStatus)
                return false;

            // 切换状态
            it.status = targetStatus;

            // 目标为 Active：补写首次创建时间
            if (targetStatus == JournalStatus.Active)
            {
                it.createdAt = DateTime.UtcNow.AddHours(-1); // 统一存 UTC，并往前移 1 小时
            }

            // 只有在明确要求时才重置步骤
            if (resetSteps && it.contents != null)
            {
                foreach (var ln in it.contents)
                {
                    if (ln?.line == null)
                        continue;
                    if (ln.line.Kind == JournalLineKind.Step)
                        ln.State = StepState.Pending;
                }
            }

            FlushJournalSnapshot();
            return true;
        }

        // 向 SettingsMgr 写回当前日记快照（是否立即落盘由 SettingsMgr 决定）
        private void FlushJournalSnapshot()
        {
            if (Model == null)
                return;
            EventBus.Publish(new EJournalUIChanged());
            var snap = JournalSaveAdapter.ToSave(Model);
            SettingsMgr.Instance.SetJournalSnapshot(snap, true);
        }
    }
}
