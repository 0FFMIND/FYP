using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MVC;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Manager
{
    [Serializable]
    public class LocalizationEntry
    {
        public string zh;
        public string en;
    }

    // JsonUtility 无法直接解析顶层数组，需要使用Helper用一个临时包装进行解析
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
        private LanguageCode CurrentLanguage = LanguageCode.zh;

        // 加载后存储 key -> [zh,en,ja]
        private Dictionary<string, string[]> table;

        // 上次加载时的场景名 & 语言，用来跳过重复加载
        private string lastSceneName;

        // 监听全局设置变更
        private void OnEnable()
        {
            EventBus.Subscribe<ESettingsChanged>(ApplyLanguage);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ESettingsChanged>(ApplyLanguage);
        }

        // 重载
        private void ApplyLanguage(ESettingsChanged e)
        {
            if (e.Settings.language == CurrentLanguage)
            {
                return;
            }
            CurrentLanguage = e.Settings.language;
            // 广播语言变更事件，驱动 UI/文本组件刷新
            EventBus.Publish(new ELanguageChanged(CurrentLanguage));
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

        public string GetText(string key, bool trim = true)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "";
            }

            // 获取当前激活场景名
            string sceneName = SceneManager.GetActiveScene().name;

            // 如果还没加载，或场景／语言换了，就重载
            if (sceneName != lastSceneName)
            {
                // 载入当前场景的string.json和全局UI的string.json
                LoadTableForScene(sceneName);
                lastSceneName = sceneName;
            }
            if (trim)
            {
                key = key.Trim();
            }

            if (table != null && table.TryGetValue(key, out var values))
            {
                // 找到 key，取对应语言栏位
                int langIndex = (int)CurrentLanguage;
                return UnescapeCommon(values[langIndex]);
            }

            // 未找到时
            return key;
        }

        private string UnescapeCommon(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            // 先处理 \r\n 组合，再处理单个
            s = s.Replace("\\r\\n", "\n");
            s = s.Replace("\\n", "\n");
            s = s.Replace("\\t", "\t");
            s = s.Replace("\\r", "\r");

            // 如确有需要再还原双反斜杠到单反斜杠
            // s = s.Replace("\\\\", "\\");
            return s;
        }

        // 从 StreamingAssets/Localization/{sceneName}/strings.json 读取表格
        private void LoadTableForScene(string sceneName)
        {
            table = new Dictionary<string, string[]>();
            string[] folders =
            {
                Path.Combine(Application.streamingAssetsPath, "Localization", sceneName),
                Path.Combine(Application.streamingAssetsPath, "Localization"),
            };
            foreach (var folder in folders)
            {
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
        }
    }
}
