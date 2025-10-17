using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using Utils; 
using UnityEngine;

namespace MVC
{
    public class JournalMgr : SingletonMB<JournalMgr>
    {
        // 内存中的日记条目列表（顺序即展示顺序）
        private readonly List<JournalItem> _items = new List<JournalItem>();
        // 当前激活的任务键（用于将上一项置为完成）
        private string _activeKey;

        public event Action OnChanged;

        private void OnEnable()
        {
            // 订阅推进事件
            EventBus.Subscribe<JournalAdvanceEvent>(OnAdvanceEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<JournalAdvanceEvent>(OnAdvanceEvent);
        }


        // 初始化“今日待办”的基础条目（可在关卡加载时调用一次）
        public void InitToday(params (string key, string title, bool done)[] seeds)
        {
            _items.Clear();
            _activeKey = null;
            foreach (var (key, title, done) in seeds)
            {
                _items.Add(new JournalItem
                {
                    key = key,
                    title = title,
                    status = done ? JournalStatus.Done : JournalStatus.Pending,
                    createdAt = DateTime.Now
                });
                if (!done && _activeKey == null) _activeKey = key;
            }
            OnChanged?.Invoke();
        }

        // 外部也可直接调用：推进到某个任务/状态
        public void AdvanceTo(string key, string title)
        {
            // 1) 把上一个激活项打勾
            if (!string.IsNullOrEmpty(_activeKey))
            {
                var prev = _items.FirstOrDefault(i => i.key == _activeKey);
                if (prev != null) prev.status = JournalStatus.Done;
            }

            // 2) 新项若不存在就追加；存在则激活并更新标题（以便支持本地化热更新）
            var cur = _items.FirstOrDefault(i => i.key == key);
            if (cur == null)
            {
                cur = new JournalItem
                {
                    key = key,
                    title = title,
                    status = JournalStatus.Active,
                    createdAt = DateTime.Now
                };
                _items.Add(cur);
            }
            else
            {
                cur.title = title;
                cur.status = JournalStatus.Active;
            }

            _activeKey = key;
            OnChanged?.Invoke();
        }

        private void OnAdvanceEvent(JournalAdvanceEvent e) => AdvanceTo(e.Key, e.Title);
    }
}
