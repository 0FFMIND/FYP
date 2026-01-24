using MVC;
using Steamworks;
/// <summary>
/// 将 Steam 的语言代码映射到游戏内的 LanguageCode
/// </summary>
public static class SteamLanguageResolver
{
    public static bool TryGetSteamGameLanguage(out LanguageCode code, out string raw)
    {
        // 默认值
        code = LanguageCode.zh;
        raw = null;

        // 检查 Steam 客户端是否在运行（否则 Steam API 调用可能无意义或失败）
        if (!SteamAPI.IsSteamRunning())
            return false;
        // 读取 Steam 当前游戏语言
        raw = SteamApps.GetCurrentGameLanguage();
        if (string.IsNullOrEmpty(raw))
            return false;

        code = MapSteamLanguage(raw);
        return true;
    }

    public static LanguageCode MapSteamLanguage(string steamLang)
    {
        steamLang = steamLang.Trim().ToLowerInvariant();

        return steamLang switch
        {
            "english" => LanguageCode.en,
            "schinese" => LanguageCode.zh,
            _ => LanguageCode.zh
        };
    }
}
