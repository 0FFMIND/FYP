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
            var userOverride = e.UserOverride;
            if (lang == _data.language && userOverride == _data.languageUserOverride)
            {
                return;
            }
            _data.language = lang;
            _data.languageUserOverride = userOverride;
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

        public void SetInventorySnapshot(InventorySaveData snap, bool saveNow = false)
        {
            if (snap == null)
            {
                return;
            }
            if (InventoryEquals(_data.inventoryData, snap))
            {
                return;
            }
            _data.inventoryData = snap;
            if (saveNow)
            {
                Save();
            }
        }

        // 写回 Journal 快照到 Settings；saveNow=true 时立刻保存（会触发 Broadcast）
        public void SetJournalSnapshot(JournalSaveData snap, bool saveNow = false)
        {
            if (snap == null)
            {
                return;
            }
            if (JournalEquals(_data.journalData, snap))
            {
                // 与现有存档一致，不必落盘
                return;
            }

            _data.journalData = snap;

            if (saveNow)
            {
                Save();
            }
        }

        public void SetChapter1Completed(bool completed)
        {
            if (_data.chapter1Completed == completed)
            {
                return;
            }
            _data.chapter1Completed = completed;
            Save();
        }

        public void SetChapter1HiddenCompleted(bool completed)
        {
            if (_data.chapter1HiddenCompleted == completed)
            {
                return;
            }
            _data.chapter1HiddenCompleted = completed;
            Save();
        }

        public void SetChapter2Completed(bool completed)
        {
            if (_data.chapter2Completed == completed)
            {
                return;
            }
            _data.chapter2Completed = completed;
            Save();
        }

        public void SetLanguageUserOverride(bool overridden)
        {
            if (_data.languageUserOverride == overridden)
            {
                return;
            }
            _data.languageUserOverride = overridden;
            Save();
        }

        public bool GetLanguageUserOverride()
        {
            return _data.languageUserOverride;
        }

        public bool GetChapter1HiddenCompleted()
        {
            return _data.chapter1HiddenCompleted;
        }

        public bool GetChapter1Completed()
        {
            return _data.chapter1Completed;
        }

        public bool GetChapter2Completed()
        {
            return _data.chapter2Completed;
        }

        public JournalSaveData GetJournalSnapshot()
        {
            return _data.journalData;
        }

        public InventorySaveData GetInventorySnapshot()
        {
            return _data.inventoryData;
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

        // 清空 Inventory（保留容量，不保留物品）
        public void ClearInventory(bool saveNow = true)
        {
            var capacity = _data?.inventoryData?.capacity ?? 99;
            var empty = new InventorySaveData
            {
                capacity = capacity,
                itemIds = new List<string>(),
                counts = new List<int>(),
            };
            SetInventorySnapshot(empty, saveNow);
        }

        // 清空 Journal（清空所有条目与 steps）
        public void ClearJournal(bool saveNow = true)
        {
            var empty = new JournalSaveData
            {
                keys = new List<string>(),
                statuses = new List<string>(),
                createdAtIso = new List<string>(),
                steps = new List<JournalItemSteps>(),
            };
            SetJournalSnapshot(empty, saveNow);
        }

        // 一键清空进度（Inventory + Journal）
        public void ClearChapter1Progress()
        {
            // 顺序无所谓，这里避免重复落盘：都传 false，最后统一 Save 或至少 Broadcast
            ClearInventory(false);
            ClearJournal(false);

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

        // 判断两个 Journal 存档是否相等（容量/顺序/元素逐一比对）
        private bool JournalEquals(JournalSaveData a, JournalSaveData b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null)
                return false;

            // 三列必须同时存在
            if (a.keys == null || a.statuses == null || a.createdAtIso == null)
                return false;
            if (b.keys == null || b.statuses == null || b.createdAtIso == null)
                return false;

            // 列长度一致
            if (a.keys.Count != b.keys.Count)
                return false;
            if (a.statuses.Count != b.statuses.Count)
                return false;
            if (a.createdAtIso.Count != b.createdAtIso.Count)
                return false;

            // 逐行比对（保持顺序稳定）
            for (int i = 0; i < a.keys.Count; i++)
            {
                if (!string.Equals(a.keys[i], b.keys[i], StringComparison.Ordinal))
                    return false;
                if (!string.Equals(a.statuses[i], b.statuses[i], StringComparison.Ordinal))
                    return false;
                if (!string.Equals(a.createdAtIso[i], b.createdAtIso[i], StringComparison.Ordinal))
                    return false;
            }
            // —— 也要比较 steps（每条目下的行状态：textKeys/indices/states）——
            // 两边 steps 的存在性与长度
            if ((a.steps == null) != (b.steps == null))
                return false;
            if (a.steps != null)
            {
                if (a.steps.Count != b.steps.Count)
                    return false;
                for (int i = 0; i < a.steps.Count; i++)
                {
                    var sa = a.steps[i];
                    var sb = b.steps[i];
                    // 任一为 null 则只有两者都 null 才相等
                    if ((sa == null) != (sb == null))
                        return false;
                    if (sa == null)
                        continue;

                    // 三列存在性与长度一致
                    if ((sa.textKeys == null) != (sb.textKeys == null))
                        return false;
                    if ((sa.indices == null) != (sb.indices == null))
                        return false;
                    if ((sa.states == null) != (sb.states == null))
                        return false;
                    int tkCount = sa.textKeys?.Count ?? 0;
                    int idCount = sa.indices?.Count ?? 0;
                    int stCount = sa.states?.Count ?? 0;
                    if (tkCount != (sb.textKeys?.Count ?? 0))
                        return false;
                    if (idCount != (sb.indices?.Count ?? 0))
                        return false;
                    if (stCount != (sb.states?.Count ?? 0))
                        return false;

                    // 逐项比对
                    for (int k = 0; k < tkCount; k++)
                        if (
                            !string.Equals(sa.textKeys[k], sb.textKeys[k], StringComparison.Ordinal)
                        )
                            return false;
                    for (int k = 0; k < idCount; k++)
                        if (sa.indices[k] != sb.indices[k])
                            return false;
                    for (int k = 0; k < stCount; k++)
                        if (!string.Equals(sa.states[k], sb.states[k], StringComparison.Ordinal))
                            return false;
                }
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
