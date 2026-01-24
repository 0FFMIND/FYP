using UnityEngine;
using Manager;
using Utils;
using MVC;
using Steamworks;

public class LanguageBootstrap : MonoBehaviour
{
    private void Awake()
    {
        // 玩家已经手动改过，不再用 Steam 覆盖
        if (SettingsMgr.Instance.GetLanguageUserOverride())
        {
            return;
        }
        // 尝试从 Steam 读取当前游戏语言
        if (SteamLanguageResolver.TryGetSteamGameLanguage(out var lang, out var raw))
        {
            // 来自 Steam 的默认值 => UserSelected = false
            EventBus.Publish(new ELanguageSet(lang, false));
        }
    }
}
