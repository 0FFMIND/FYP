using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Utils.SingletonPattern;

namespace Manager
{
    public class SettingsMgr : SingletonMB<SettingsMgr>
    {
        // 可被序列化
        [Serializable]
        // 默认值
        public class SettingsDTO
        {
            // 用的分贝(db)表示
            public float bgmVolume = 0f;
            public float sfxVolume = 0f;
            public string language = "en";

            // 用dictionary方便查找
            public Dictionary<InputAction, KeyCode> keyBindings = new()
            {
                { InputAction.DialogueClick, KeyCode.Return },
                { InputAction.PlayerSprint, KeyCode.LeftShift },
                { InputAction.PauseGame, KeyCode.Escape },
            };
        }

        // 默认保存路径，明文保存
        private static string SettingsFilePath =>
            Path.Combine(Application.persistentDataPath, "settings.json");
        // 外部订阅事件(当有设置被载入的时候应用修改)
        public event Action<SettingsDTO> SettingsChanged;
        private SettingsDTO Data;

        // AutoSingletonMB
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            EnsureCreated();
        }
        private void Awake()
        {
            Load();
        }

        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Data, Formatting.Indented);
                File.WriteAllText(SettingsFilePath, json);
                Debug.Log($"[SettingsMgr] 保存成功: {SettingsFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SettingsMgr] 保存失败: {e}");
            }
        }

        public void Load()
        {
            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    Data = JsonConvert.DeserializeObject<SettingsDTO>(json);
                    Debug.Log($"[SettingsMgr] 加载成功: {SettingsFilePath}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SettingsMgr] 加载失败，使用默认值: {e}");
                    Data = new SettingsDTO();
                }
            }
            else
            {
                Debug.Log("[SettingsMgr] 未找到配置文件，使用默认设置");
                Data = new SettingsDTO();
                Save(); // 初次写入
            }
            // 广播变动
            Broadcast();
        }
        private void Broadcast()
        {
            SettingsChanged?.Invoke(Data);
        }

        /// <summary>
        /// Unity 应用退出时自动调用
        /// </summary>
        private void OnApplicationQuit()
        {
            Save();
        }
    }
}
