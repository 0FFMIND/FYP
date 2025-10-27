using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MVC
{
    [Serializable]
    public class JournalModel
    {
        private static readonly DateTime DEFAULT_TIME = DateTime.MinValue;

        // 运行时的日记条目顺序表：保持加载时（或追加时）的顺序
        private readonly List<JournalItem> _items = new();

        // 只读访问
        public IReadOnlyList<JournalItem> RawItems => _items;

        // key → JournalItem 的索引表
        private readonly Dictionary<string, JournalItem> _byKey = new(StringComparer.Ordinal);

        // 通过 key 查找JounralItem
        public JournalItem Find(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            // 命中则返回条目，否则返回 null
            return _byKey.TryGetValue(key, out var it) ? it : null;
        }

        // 将 Resources/SO/Journals 下的所有 JournalData 载入并实例化为运行态对象
        public void LoadFromSO()
        {
            _items.Clear();
            _byKey.Clear();

            var assets = Resources.LoadAll<JournalData>("SO/Journals");
            // 未找到任何资产
            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning(
                    $"[JournalModel] No JournalData assets found at Resources/SO/Journals"
                );
                return;
            }

            // 遍历所有加载到的 ScriptableObject 资产
            foreach (var so in assets)
            {
                if (!so || string.IsNullOrEmpty(so.key))
                    continue;
                // 同一 key 出现多次（资源重复）
                if (_byKey.ContainsKey(so.key))
                {
                    Debug.LogWarning($"[JournalModel] Duplicate journal key '{so.key}' ignored.");
                    continue;
                }

                // 从 SO 生成运行态 Item（默认 Hidden + MinValue）
                var item = new JournalItem
                {
                    key = so.key,
                    title = so.title,
                    status = JournalStatus.Hidden,
                    createdAt = DEFAULT_TIME,
                    contents = new List<JournalLine>(so.contents?.Count ?? 0),
                };

                // 深拷贝每一行，避免运行时改到 SO
                if (so.contents != null)
                {
                    foreach (var line in so.contents)
                    {
                        if (line == null) continue;
                        item.contents.Add(new JournalLine
                        {
                            line = line,
                            State = StepState.Pending,
                        });
                    }
                }

                _items.Add(item);
                _byKey[item.key] = item;
            }
        }
    }
}
