using System;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;

namespace MVC
{
    public class ScreenModeCtl : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text currentScreenText;

        private List<FullScreenMode> _modeList = new List<FullScreenMode>();
        private int _index;

        private void Awake()
        {
            // 若未在 Inspector 赋值
            if (!currentScreenText)
            {
                Debug.LogError($"[ScreenModeCtl] 未挂载 TMP_Text（{name}）");
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            // 构建并排序屏幕列表
            BuildModeList();
            var currentMode = SettingsMgr.Instance.GetScreenMode();
            // 找到与保存分辨率最接近的列表索引
            _index = FindIndexClosest(currentMode);
            // 刷新 UI 文本
            ApplyAndUpdateText(applyNow: false);
        }

        public void SwitchLeft()
        {
            if (_modeList.Count == 0)
                return;
            _index = (_index - 1 + _modeList.Count) % _modeList.Count;
            ApplyAndUpdateText(applyNow: true);
        }

        public void SwitchRight()
        {
            if (_modeList.Count == 0)
                return;
            _index = (_index + 1) % _modeList.Count;
            ApplyAndUpdateText(applyNow: true);
        }

        // ========== 内部实现 ==========

        private void BuildModeList()
        {
            _modeList.Clear();
            _modeList.Add(FullScreenMode.FullScreenWindow);
            _modeList.Add(FullScreenMode.Windowed);
        }

        private int FindIndexClosest(FullScreenMode mode)
        {
            for (int i = 0; i < _modeList.Count; i++)
            {
                var v = _modeList[i];
                if(v == mode)
                {
                    return i;
                }
            }
            return -1;
        }

        private void ApplyAndUpdateText(bool applyNow)
        {
            // 应用当前分辨率，并通知系统更新
            var v = _modeList[_index];
            string text = "";
            if (v == FullScreenMode.Windowed)
            {
                text = "窗口化";
            }
            else if (v == FullScreenMode.FullScreenWindow)
            {
                text = "全屏";
            }
            // 显示文本
            currentScreenText.text = text;

            if (!applyNow)
                return;

            SettingsMgr.Instance.SetScreenMode(v);
        }
    }
}
