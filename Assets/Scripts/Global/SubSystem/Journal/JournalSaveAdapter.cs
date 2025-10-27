using System;
using System.Collections.Generic;
using System.Globalization;
using MVC;

// 允许被 JSON/Unity 序列化
[Serializable]
public class JournalSaveData
{
    // 每条目的 key
    public List<string> keys = new();

    // 当前状态"Hidden"|"Active"|"Completed"
    public List<string> statuses = new();

    // 创建时间，ISO8601（UTC），未 Reveal 用空字符串
    public List<string> createdAtIso = new();

    // 每个条目一份行状态表
    public List<JournalItemSteps> steps = new();
}

// 存每个条目的“步骤行”状态；只记录 Kind==Step 的行
[Serializable]
public class JournalItemSteps
{
    public List<string> textKeys = new(); // 每条Step的TextKey（主匹配字段，可为空）
    public List<int> indices = new(); // 兜底：该Step在contents里的索引
    public List<string> states = new(); // "Pending" | "Done"
}

public static class JournalSaveAdapter
{
    // 将运行时模型转换为可序列化的存档数据（不改动模型本身）
    public static JournalSaveData ToSave(JournalModel m)
    {
        var d = new JournalSaveData();
        // 遍历模型的原始顺序条目（保持稳定的写出顺序）
        foreach (var it in m.RawItems)
        {
            d.keys.Add(it.key);
            d.statuses.Add(it.status.ToString());
            d.createdAtIso.Add(
                it.createdAt == DateTime.MinValue
                    ? ""
                    : it.createdAt.ToString("o", CultureInfo.InvariantCulture)
            );
            // 行级：只写出 Step 行
            var lines = new JournalItemSteps();
            if (it.contents != null)
            {
                for (int i = 0; i < it.contents.Count; i++)
                {
                    var ln = it.contents[i];
                    if (ln?.line == null)
                        continue;
                    if (ln.line.Kind != JournalLineKind.Step)
                        continue;

                    lines.textKeys.Add(ln.line.TextKey ?? "");
                    lines.indices.Add(i);
                    lines.states.Add(ln.State.ToString()); // "Pending"/"Done"
                }
            }
            d.steps.Add(lines);
        }
        return d;
    }

    // 将存档状态应用到运行时模型
    public static void ApplyToModel(JournalModel m, JournalSaveData d)
    {
        if (m == null || d == null)
            return;

        int n = Math.Min(Math.Min(d.keys.Count, d.statuses.Count), d.createdAtIso.Count);
        // 若 steps 数量不足，按可用的最小值
        n = Math.Min(n, d.steps?.Count ?? 0);
        for (int i = 0; i < n; i++)
        {
            // 取出该行的 key
            string key = d.keys[i];
            // 在模型中查找对应条目
            var it = m.Find(key);
            if (it == null)
                continue;

            // 恢复状态：字符串到枚举的宽松解析（忽略大小写）
            if (Enum.TryParse(d.statuses[i], true, out JournalStatus st))
                it.status = st;

            // 恢复创建时间：空串表示未 Reveal；否则尝试解析为 UTC
            string iso = d.createdAtIso[i];
            if (
                !string.IsNullOrEmpty(iso)
                && DateTime.TryParse(
                    iso,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var dt
                )
            )
            {
                it.createdAt = dt.ToUniversalTime();
            }
            else
            {
                // 空字符串：代表未 Reveal
                it.createdAt = DateTime.MinValue;
            }
            // 行级状态（只回放 Step）
            var savedLines = d.steps[i];
            if (savedLines != null && it.contents != null)
            {
                // 1) 先按 TextKey 精确匹配
                var mapByKey = new Dictionary<string, string>(StringComparer.Ordinal);
                for (int k = 0; k < savedLines.textKeys.Count && k < savedLines.states.Count; k++)
                {
                    var tk = savedLines.textKeys[k] ?? "";
                    mapByKey[tk] = savedLines.states[k];
                }

                for (int j = 0; j < it.contents.Count; j++)
                {
                    var ln = it.contents[j];
                    if (ln?.line == null || ln.line.Kind != JournalLineKind.Step)
                        continue;

                    var tk = ln.line.TextKey ?? "";
                    if (
                        mapByKey.TryGetValue(tk, out var stStr)
                        && Enum.TryParse(stStr, true, out StepState stepSt)
                    )
                    {
                        ln.State = stepSt;
                        // 已按TextKey命中则跳过索引兜底
                        mapByKey.Remove(tk);
                    }
                }

                // 2) 兜底：按索引（仅对那些未被TextKey命中的保存项生效）
                int cnt = Math.Min(
                    Math.Min(savedLines.indices.Count, savedLines.states.Count),
                    savedLines.textKeys.Count
                );
                for (int k = 0; k < cnt; k++)
                {
                    int idx = savedLines.indices[k];
                    string stStr = savedLines.states[k];

                    // 若该保存项已被TextKey流程命中，则跳过
                    var tk = savedLines.textKeys[k] ?? "";
                    if (mapByKey.ContainsKey(tk) == false)
                        continue; // 已命中并移除

                    if (idx < 0 || idx >= (it.contents?.Count ?? 0))
                        continue;
                    var ln = it.contents[idx];
                    if (ln?.line == null || ln.line.Kind != JournalLineKind.Step)
                        continue;

                    if (Enum.TryParse(stStr, true, out StepState stepSt))
                        ln.State = stepSt;
                }
            }
        }
    }
}
