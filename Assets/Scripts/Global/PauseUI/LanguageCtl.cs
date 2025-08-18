using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanguageCtl : MonoBehaviour
{
    [SerializeField]
    private TMP_Text currentLanguageText;
    private string[] languages = { "zh", "en" };
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
        var cur = LocalizationMgr.Instance.CurrentLanguage;
        index = 0;
        for (int i = 0; i < languages.Length; i++)
        {
            if (string.Equals(languages[i], cur, StringComparison.OrdinalIgnoreCase))
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
        LocalizationMgr.Instance.SetLanguage(code);
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

    private string DisplayName(string code)
    {
        // 将语言代码转换为可读名称
        switch (code.ToLowerInvariant())
        {
            case "zh":
                return "简体中文";
            case "en":
                return "English";
            default:
                return code;
        }
    }
}
