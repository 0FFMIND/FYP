using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
                    status = JournalStatus.Hidden,
                    createdAt = DEFAULT_TIME,
                    contents = new List<JournalLine>(so.contents?.Count ?? 0),
                };

                // 深拷贝每一行，避免运行时改到 SO
                if (so.contents != null)
                {
                    foreach (var line in so.contents)
                    {
                        if (line == null)
                            continue;
                        TrimCRInObjectStrings(line);
                        item.contents.Add(
                            new JournalLine { line = line, State = StepState.Pending }
                        );
                    }
                }

                _items.Add(item);
                _byKey[item.key] = item;
            }
        }

        // 修剪“每一行 content”里的所有 string 字段/属性（排除名为 key/title 的成员）
        private static void TrimCRInObjectStrings(object obj)
        {
            if (obj == null)
                return;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            foreach (var f in obj.GetType().GetFields(flags))
            {
                if (f.FieldType == typeof(string) && !IsKeyOrTitleName(f.Name))
                {
                    var val = (string)f.GetValue(obj);
                    var trimmed = TrimCR(val);
                    if (!ReferenceEquals(val, trimmed))
                        f.SetValue(obj, trimmed);
                }
            }
            foreach (var p in obj.GetType().GetProperties(flags))
            {
                if (!p.CanRead || !p.CanWrite)
                    continue;
                if (p.PropertyType == typeof(string) && !IsKeyOrTitleName(p.Name))
                {
                    var val = (string)p.GetValue(obj, null);
                    var trimmed = TrimCR(val);
                    if (!ReferenceEquals(val, trimmed))
                        p.SetValue(obj, trimmed, null);
                }
            }
        }

        private static bool IsKeyOrTitleName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            var n = name.ToLowerInvariant();
            return n == "key" || n == "title";
        }

        private static string TrimCR(string s) => s?.Trim('\r');

        // Debug用
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"JournalModel: {_items.Count} items");

            for (int i = 0; i < _items.Count; i++)
            {
                var it = _items[i];
                string created =
                    it.createdAt == DEFAULT_TIME
                        ? "-"
                        : it.createdAt.ToString("o", CultureInfo.InvariantCulture);

                // 统计本条目的 Step 完成度
                int stepDone = 0,
                    stepPending = 0,
                    stepTotal = 0;
                if (it.contents != null)
                {
                    foreach (var c in it.contents)
                    {
                        if (c?.line == null)
                            continue;
                        if (c.line.Kind == JournalLineKind.Step)
                        {
                            stepTotal++;
                            if (c.State == StepState.Done)
                                stepDone++;
                            else if (c.State == StepState.Pending)
                                stepPending++;
                        }
                    }
                }

                sb.AppendLine(
                    $"[{i}] key={it.key} | status={it.status} | createdAt={created} | steps {stepDone}/{stepTotal} done"
                );

                // 逐行明细
                if (it.contents != null)
                {
                    for (int j = 0; j < it.contents.Count; j++)
                    {
                        var ln = it.contents[j];
                        if (ln == null)
                        {
                            sb.AppendLine($"    ({j}) <null>");
                            continue;
                        }
                        if (ln.line == null)
                        {
                            sb.AppendLine($"    ({j}) <line:null> state={ln.State}");
                            continue;
                        }

                        var kind = ln.line.Kind;
                        var tk = ln.line.TextKey ?? "";

                        // 只有 Step 才有意义的完成状态；Fixed 显示 "-"
                        string stateStr =
                            (kind == JournalLineKind.Step) ? ln.State.ToString() : "-";

                        sb.AppendLine($"    ({j}) {kind} | tk=\"{Esc(tk)}\" | state={stateStr}");
                    }
                }
            }
            return sb.ToString();
            // --- helpers ---
            static string Esc(string s) =>
                (s ?? "")
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n");
        }
    }
}
