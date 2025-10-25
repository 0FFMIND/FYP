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
        }
        return d;
    }

    // 将存档状态应用到运行时模型
    public static void ApplyToModel(JournalModel m, JournalSaveData d)
    {
        if (m == null || d == null)
            return;

        int n = Math.Min(Math.Min(d.keys.Count, d.statuses.Count), d.createdAtIso.Count);
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
        }
    }
}
