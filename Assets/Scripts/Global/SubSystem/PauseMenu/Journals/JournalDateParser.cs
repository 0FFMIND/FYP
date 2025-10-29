using System;
using System.Collections.Generic;
using System.Globalization;

public static class JournalDateParser
{
    // 将 DateTime(可空) 解析为用于本地化模板的占位符字典：yyyy/MM/dd/hh/mm/period
    public static Dictionary<string, string> ParseToArgs(DateTime? dtOpt)
    {
        // 预置键，避免缺参日志
        var dict = new Dictionary<string, string>
        {
            ["yyyy"] = "",
            ["MM"] = "",
            ["dd"] = "",
            ["hh"] = "",
            ["mm"] = "",
            ["period"] = ""
        };

        if (!dtOpt.HasValue) return dict;

        var dt = dtOpt.Value;
        dt = dt.ToLocalTime();
        // 24 小时制的小时
        int h24 = dt.Hour;
        // 转 12 小时制
        int h12 = h24 % 12; if (h12 == 0) h12 = 12;

        // 使用 InvariantCulture 保证数字格式固定宽度和不受系统语言影响
        dict["yyyy"] = dt.Year.ToString("0000", CultureInfo.InvariantCulture);
        dict["MM"] = dt.Month.ToString("00", CultureInfo.InvariantCulture);
        dict["dd"] = dt.Day.ToString("00",CultureInfo.InvariantCulture);
        dict["hh"] = h12.ToString("00", CultureInfo.InvariantCulture);
        dict["mm"] = dt.Minute.ToString("00", CultureInfo.InvariantCulture);
        string periodZh;
        if (h24 < 6) periodZh = "凌晨";
        else if (h24 < 12) periodZh = "上午";
        else if (h24 < 18) periodZh = "下午";
        else periodZh = "夜间";
        dict["period"] = periodZh;

        return dict;
    }
}
