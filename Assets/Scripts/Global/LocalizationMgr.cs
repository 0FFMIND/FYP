using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.SingletonPattern;

namespace Manager
{
    [Serializable]
    public class LocalizationEntry
    {
        public string zh;
        public string en;
    }

    public static class JsonHelper
    {
        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }

        public static T[] FromJsonArray<T>(string json)
        {
            // 在前后补上 {"array": …}
            string wrapped = "{\"array\":" + json + "}";
            return JsonUtility.FromJson<Wrapper<T>>(wrapped).array;
        }
    }

    public class LocalizationTableWrapper
    {
        public LocalizationEntry[] entries;
    }

    public class LocalizationMgr : SingletonMB<LocalizationMgr>
    {
        public string CurrentLanguage = "zh";

        // 加载后存储 key -> [zh,en,ja]
        private Dictionary<string, string[]> table;

        // 上次加载时的场景名 & 语言，用来跳过重复加载
        private string lastSceneName;
        private string lastLanguage;

        public void SetLanguage(string lanCode)
        {
            if (lanCode == CurrentLanguage)
                return;
            CurrentLanguage = lanCode;
            // 强制下次重新加载
            table = null;
        }

        public string GetDialoguePath(string sceneName, string filename)
        {
            return Path.Combine(
                Application.streamingAssetsPath,
                "Localization",
                sceneName,
                filename
            );
        }

        public string GetText(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "";
            }

            // 获取当前激活场景名
            string sceneName = SceneManager.GetActiveScene().name;

            // 如果还没加载，或场景／语言换了，就重载
            if (table == null || sceneName != lastSceneName || CurrentLanguage != lastLanguage)
            {
                LoadTableForScene(sceneName);
                lastSceneName = sceneName;
                lastLanguage = CurrentLanguage;
            }

            if (table != null && table.TryGetValue(key, out var values))
            {
                // 找到 key，取对应语言栏位
                int langIndex = LanguageIndex(CurrentLanguage, values.Length);
                return values[langIndex];
            }

            // 未找到时
            return key;
        }

        // 从 StreamingAssets/Localization/{sceneName}/strings.json 读取表格
        private void LoadTableForScene(string sceneName)
        {
            table = new Dictionary<string, string[]>();
            string folder = Path.Combine(
                Application.streamingAssetsPath,
                "Localization",
                sceneName
            );
            string jsonPath = Path.Combine(folder, "strings.json");

            string json = File.ReadAllText(jsonPath);
            // 反序列化顶层数组
            var entries = JsonHelper.FromJsonArray<LocalizationEntry>(json);
            if (entries == null)
                return;
            // 按 {zh, en, ja} 顺序填入字典
            foreach (var e in entries)
            {
                table[e.zh] = new string[]
                {
                    e.zh,
                    e.en, /*, e.ja */
                };
            }
        }

        private int LanguageIndex(string langCode, int maxLength)
        {
            switch (langCode.ToLower())
            {
                case "zh":
                    return 0;
                case "en":
                    return 1;
                case "ja":
                    return 2;
                // 加其他语言时在这里扩展
                default:
                    return 0;
            }
        }
    }
}
