using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace MVC
{
    public enum SettingField
    {
        BgmVolume,
        SfxVolume,
        MixerVolume,
        PlayerSpeed,
        TypeSpeed,
        SprintMultiplier,
        Language,
        KeyBindings,
    }

    /// <summary>
    /// 纯数据载体（Data-Only）：保存游戏设置的当前值，用于序列化/反序列化与快照传递
    /// 任何业务逻辑（校验/冲突检测）存在 SettingsModel 中
    /// </summary>
    // 可被序列化
    [Serializable]
    public class SettingsData
    {
        // 用的分贝(db)表示
        public float bgmVolume = 0f;
        public float sfxVolume = 0f;
        public float mixerVolume = 0f;

        public float playerSpeed = 3f;
        public float typeSpeed = 0.08f;
        public float sprintMultiplier = 2.5f;
        public LanguageCode language = LanguageCode.zh;

        public int screenWidth = 1920;
        public int screenHeight = 1080;
        public FullScreenMode screenMode = FullScreenMode.FullScreenWindow;

        // 章节完成状态
        public bool chapter1Completed = false;
        public bool chapter1HiddenCompleted = false;
        public bool chapter2Completed = false;

        // 用dictionary方便查找
        public Dictionary<InputAction, KeyCode> keyBindings = new()
        {
            { InputAction.DialogueClick, KeyCode.Return },
            { InputAction.PlayerSprint, KeyCode.LeftShift },
            { InputAction.PauseGame, KeyCode.Escape },
        };
        public InventorySaveData inventoryData = new InventorySaveData { capacity = 99 };

        public JournalSaveData journalData = new JournalSaveData();


        /// <summary>
        /// 生成一份深拷贝
        /// - 避免外部持有对内部的引用而绕过模型层直接修改；
        /// </summary>
        public SettingsData DeepCopy()
        {
            return new SettingsData
            {
                bgmVolume = bgmVolume,
                sfxVolume = sfxVolume,
                mixerVolume = mixerVolume,
                language = language,
                playerSpeed = playerSpeed,
                sprintMultiplier = sprintMultiplier,
                typeSpeed = typeSpeed,
                screenHeight = screenHeight,
                screenWidth = screenWidth,
                screenMode = screenMode,
                chapter1Completed = chapter1Completed,
                chapter2Completed = chapter2Completed,
                chapter1HiddenCompleted = chapter1HiddenCompleted,
                keyBindings = new Dictionary<InputAction, KeyCode>(keyBindings),
                inventoryData = new InventorySaveData
                {
                    capacity = inventoryData.capacity,
                    itemIds = new List<string>(inventoryData.itemIds),
                    counts = new List<int>(inventoryData.counts),
                },
                journalData = new JournalSaveData
                {
                    keys = new List<string>(journalData.keys),
                    statuses = new List<string>(journalData.statuses),
                    createdAtIso = new List<string>(journalData.createdAtIso),
                    steps = new List<JournalItemSteps>(journalData.steps),
                }
            };
        }
    }
}
