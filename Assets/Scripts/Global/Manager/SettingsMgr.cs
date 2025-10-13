using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MVC;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace Manager
{
    public class SettingsMgr : SingletonMB<SettingsMgr>
    {
        // 默认保存路径，明文保存
        private static string SettingsFilePath =>
            Path.Combine(Application.persistentDataPath, "settings.json");

        private SettingsData _data;

        private void Awake()
        {
            Load();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ELanguageSet>(SetLanguage);
            EventBus.Subscribe<EVolumeSet>(SetVolume);
            EventBus.Subscribe<EKeySet>(SetKey);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ELanguageSet>(SetLanguage);
            EventBus.Unsubscribe<EVolumeSet>(SetVolume);
            EventBus.Unsubscribe<EKeySet>(SetKey);
        }

        // 返回副本给外部
        public SettingsData Snapshot() => _data.DeepCopy();

        public void Save()
        {
            try
            {
                // 获取模型快照
                var snapshot = _data.DeepCopy();
                var json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
                File.WriteAllText(SettingsFilePath, json);
                Debug.Log($"[SettingsMgr] 保存成功: {SettingsFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SettingsMgr] 保存失败: {e}");
            }
            Broadcast(Snapshot());
        }

        public void Load()
        {
            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    _data = JsonConvert.DeserializeObject<SettingsData>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SettingsMgr] 加载失败，使用默认值: {e}");
                    _data = new SettingsData();
                }
            }
            else
            {
                Debug.Log("[SettingsMgr] 未找到配置文件，使用默认设置");
                _data = new SettingsData();
            }
            // 应用系统设置
            ApplyDisplaySettings();
            // 广播变动
            Broadcast(Snapshot());
        }

        public void ApplyDisplaySettings()
        {
            Screen.fullScreenMode = _data.screenMode;
            Screen.SetResolution(_data.screenWidth, _data.screenHeight, _data.screenMode);
        }

        public void ResetToDefaults(SettingField[] fields)
        {
            foreach (var field in fields)
            {
                if (field == SettingField.BgmVolume)
                {
                    _data.bgmVolume = 0f;
                }
                if (field == SettingField.SfxVolume)
                {
                    _data.sfxVolume = 0f;
                }
                if (field == SettingField.MixerVolume)
                {
                    _data.mixerVolume = 0f;
                }
                if (field == SettingField.PlayerSpeed)
                {
                    _data.playerSpeed = 3f;
                }
                if (field == SettingField.TypeSpeed)
                {
                    _data.typeSpeed = 0.08f;
                }
                if (field == SettingField.SprintMultiplier)
                {
                    _data.sprintMultiplier = 2.5f;
                }
                if (field == SettingField.KeyBindings)
                {
                    _data.keyBindings = new Dictionary<InputAction, KeyCode>
                    {
                        { InputAction.DialogueClick, KeyCode.Return },
                        { InputAction.PlayerSprint, KeyCode.RightShift },
                        { InputAction.PauseGame, KeyCode.Escape },
                    };
                }
            }
            Save();
        }

        private void Broadcast(SettingsData data)
        {
            EventBus.Publish(new ESettingsChanged(data));
        }

        // 暴露获取Data成员的方法
        public Dictionary<InputAction, KeyCode> GetKeyBindings()
        {
            return _data.keyBindings;
        }

        // 暴露修改Data成员的方法
        public void SetVolume(EVolumeSet e)
        {
            if (e.Type == VolumeType.bgm)
            {
                if (e.Db == _data.bgmVolume)
                {
                    return;
                }
                _data.bgmVolume = e.Db;
            }
            else if (e.Type == VolumeType.sfx)
            {
                if (e.Db == _data.sfxVolume)
                {
                    return;
                }
                _data.sfxVolume = e.Db;
            }
            else if (e.Type == VolumeType.mixer)
            {
                if (e.Db == _data.mixerVolume)
                {
                    return;
                }
                _data.mixerVolume = e.Db;
            }
            Save();
        }

        public void SetLanguage(ELanguageSet e)
        {
            var lang = e.Language;
            if (lang == _data.language)
            {
                return;
            }
            _data.language = lang;
            Save();
        }

        public void SetResolution(int width, int height)
        {
            if (width == _data.screenWidth && height == _data.screenHeight)
            {
                return;
            }
            _data.screenWidth = width;
            _data.screenHeight = height;
            ApplyDisplaySettings();
            Save();
        }

        public void SetScreenMode(FullScreenMode mode)
        {
            if (mode == _data.screenMode)
            {
                return;
            }
            _data.screenMode = mode;
            ApplyDisplaySettings();

            Save();
        }

        public void SetInventorySnapshot(InventorySaveData snap, bool saveNow = false)
        {
            if (snap == null)
            {
                return;
            }
            if (InventoryEquals(_data.inventory, snap))
            {
                return;
            }
            _data.inventory = snap;
            if (saveNow)
            {
                Save();
            }
        }

        public InventorySaveData GetInventorySnapshot()
        {
            return _data.inventory;
        }

        public (int w, int h) GetResolution()
        {
            return (_data.screenWidth, _data.screenHeight);
        }

        public FullScreenMode GetScreenMode()
        {
            return _data.screenMode;
        }

        public LanguageCode GetLanguage()
        {
            return _data.language;
        }

        // 获取音量
        public float GetBGMVolume()
        {
            return _data.bgmVolume;
        }

        public float GetSFXVolume()
        {
            return _data.sfxVolume;
        }

        public float GetMixerVolume()
        {
            return _data.mixerVolume;
        }

        public float GetPlayerSpeed()
        {
            return _data.playerSpeed;
        }

        public float GetTypeSpeed()
        {
            return _data.typeSpeed;
        }

        public float GetSprintMultiplier()
        {
            return _data.sprintMultiplier;
        }

        public void SetPlayerSpeed(float speed)
        {
            if (speed == _data.playerSpeed)
            {
                return;
            }
            _data.playerSpeed = speed;
            Save();
        }

        public void SetTypeSpeed(float speed)
        {
            if (speed == _data.typeSpeed)
            {
                return;
            }
            _data.typeSpeed = speed;
            Save();
        }

        public void SetSprintMultiplier(float speed)
        {
            if (speed == _data.sprintMultiplier)
            {
                return;
            }
            _data.sprintMultiplier = speed;
            Save();
        }

        public void SetKey(EKeySet e)
        {
            if (_data.keyBindings[e.Action] == e.Key)
            {
                return;
            }
            _data.keyBindings[e.Action] = e.Key;
            Save();
        }

        private bool InventoryEquals(InventorySaveData a, InventorySaveData b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null)
                return false;
            if (a.capacity != b.capacity)
                return false;
            if (a.itemIds == null || b.itemIds == null || a.counts == null || b.counts == null)
                return false;
            if (a.itemIds.Count != b.itemIds.Count || a.counts.Count != b.counts.Count)
                return false;
            for (int i = 0; i < a.itemIds.Count; i++)
            {
                if (!string.Equals(a.itemIds[i], b.itemIds[i], StringComparison.Ordinal))
                    return false;
                if (a.counts[i] != b.counts[i])
                    return false;
            }
            return true;
        }

        // Unity 应用退出时自动调用
        private void OnApplicationQuit()
        {
            Save();
        }
    }
}
