using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MVC
{
    public class PlayerSettingsCtl : MonoBehaviour
    {
        [SerializeField]
        private Scrollbar moveScrollBar;

        [SerializeField]
        private Scrollbar sprintScrollBar;

        private float NormalizedMoveSpeed(float moveSpeed)
        {
            return Mathf.Clamp01(Mathf.InverseLerp(1f, 5f, moveSpeed));
        }
        private float NormalizedSprintMultiplier(float sprintMultiplier)
        {
            return Mathf.Clamp01(Mathf.InverseLerp(2f, 5f, sprintMultiplier));
        }
        private float UnnormalizedMoveSpeed(float normalized)
        {
            return Mathf.Lerp(1f, 5f, Mathf.Clamp01(normalized));
        }
        private float UnnormalizedSprintMultiplier(float normalized)
        {
            return Mathf.Lerp(2f, 5f, Mathf.Clamp01(normalized));
        }

        private void OnEnable()
        {
            // 初始化速度为当前值
            float moveSpeed = SettingsMgr.Instance.GetPlayerSpeed();
            moveScrollBar.SetValueWithoutNotify(NormalizedMoveSpeed(moveSpeed));
            float sprintMultiplier = SettingsMgr.Instance.GetSprintMultiplier();
            sprintScrollBar.SetValueWithoutNotify(NormalizedSprintMultiplier(sprintMultiplier));
        }

        public void HandleMoveSpeedChange()
        {
            // 当速度滑条数值改变时，把最新值写回SettingsMgr
            float value = moveScrollBar.value;
            SettingsMgr.Instance.SetPlayerSpeed(UnnormalizedMoveSpeed(value));
        }

        public void HandleSprintMultiplierChange()
        {
            // 当疾跑滑条数值改变时，把最新值写回 AudioManager
            float value = sprintScrollBar.value;
            SettingsMgr.Instance.SetSprintMultiplier(UnnormalizedSprintMultiplier(value));
        }
    }
}

