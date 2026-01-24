using UnityEngine;
using Steamworks;
using Utils;

namespace Manager
{
    /// <summary>
    /// Steam 初始化引导单例：
    /// - 自动在首场景加载前创建（依赖你的 AutoSingletonBoot）
    /// - Awake 时尝试 SteamAPI.Init()
    /// - Update 时 RunCallbacks()
    /// - 程序退出时 Shutdown()
    /// </summary>
    public class SteamMgr : SingletonMB<SteamMgr>
    {
        public bool Initialized { get; private set; }

        private void Awake()
        {
            // 防止场景里重复创建导致重复 Init
            if (Initialized) return;

            // Steam 客户端未运行：直接失败（比如没开 Steam）
            if (!SteamAPI.IsSteamRunning())
            {
                Debug.LogWarning("[SteamMgr] Steam client is not running.");
                Initialized = false;
                return;
            }

            // 尝试初始化 Steamworks
            Initialized = SteamAPI.Init();

            if (!Initialized)
            {
                Debug.LogError("[SteamMgr] SteamAPI.Init failed");
                return;
            }

            // 打印信息用于确认已连接
            Debug.Log($"[SteamMgr] AppID={SteamUtils.GetAppID().m_AppId}");
        }

        private void Update()
        {
            if (Initialized)
                SteamAPI.RunCallbacks();
        }

        private void OnApplicationQuit()
        {
            if (Initialized)
            {
                SteamAPI.Shutdown();
                Initialized = false;
            }
        }
    }
}
