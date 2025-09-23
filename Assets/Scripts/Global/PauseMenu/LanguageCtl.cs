using System;
using Manager;
using TMPro;
using UnityEngine;
using Utils;

namespace MVC
{
    public enum LanguageCode
    {
        zh,
        en,
    }

    public class LanguageCtl : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text currentLanguageText;
        private LanguageCode[] languages = { LanguageCode.zh, LanguageCode.en };
        private int index;

        private void Awake()
        {
            // 初始化检查：如果没挂载 TMP_Text，就报错并禁用脚本
            if (!currentLanguageText)
            {
                Debug.LogError($"[LanguageCtl] 未挂载 TMP_Text 组件，请检查对象: {name}");
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            // 启用时同步到当前语言，确保 index 正确
            var cur = SettingsMgr.Instance.GetLanguage();
            index = 0;
            for (int i = 0; i < languages.Length; i++)
            {
                if (languages[i] == cur)
                {
                    index = i;
                    break;
                }
            }
            UpdateText();
        }

        public void SwitchLeft()
        {
            // 向左切换语言（上一个）
            index = (index - 1 + languages.Length) % languages.Length;
            ApplyLanguage();
        }

        public void SwitchRight()
        {
            // 向右切换语言（下一个）
            index = (index + 1) % languages.Length;
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            // 应用当前语言，并通知系统更新
            var code = languages[index];
            EventBus.Publish(new ELanguageSet(code));
            UpdateText();
        }

        private void UpdateText()
        {
            if (!currentLanguageText)
            {
                Debug.LogError($"[LanguageCtl] 未挂载 TMP_Text 组件，请检查对象: {name}");
                enabled = false;
                return;
            }
            currentLanguageText.text = DisplayName(languages[index]);
        }

        private string DisplayName(LanguageCode code)
        {
            // 将语言代码转换为可读名称
            switch (code)
            {
                case LanguageCode.zh:
                    return "简体中文";
                case LanguageCode.en:
                    return "English";
                default:
                    return ToCode(code);
            }
        }

        private static string ToCode(LanguageCode l)
        {
            switch (l)
            {
                case LanguageCode.zh:
                    return "zh";
                case LanguageCode.en:
                    return "en";
                default:
                    return "??";
            }
        }
    }
}
