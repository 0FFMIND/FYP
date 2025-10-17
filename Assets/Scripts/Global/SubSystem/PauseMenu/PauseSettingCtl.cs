using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    public class PauseSettingCtl : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] settingPages;
        public int PageCount => settingPages?.Length ?? 0;
        private int _settingIndex = 0;

        private void OnEnable()
        {
            ShowSettingPage(_settingIndex);
        }

        public void SwitchLeft()
        {
            // 向左切换界面（上一个）
            _settingIndex = (_settingIndex - 1 + PageCount) % PageCount;
            ShowSettingPage(_settingIndex);
        }

        public void SwitchRight()
        {
            // 向右切换界面（下一个）
            _settingIndex = (_settingIndex + 1) % PageCount;
            ShowSettingPage(_settingIndex);
        }

        public void ShowSettingPage(int i)
        {
            // 只显示第 i 页，其他页全部隐藏
            for (int k = 0; k < settingPages.Length; k++)
            {
                if (settingPages[k])
                {
                    settingPages[k].SetActive(k == i);
                }
            }
        }
    }
}
