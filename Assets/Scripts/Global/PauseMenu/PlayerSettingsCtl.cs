using System.Collections;
using System.Collections.Generic;
using Manager;
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

        [SerializeField]
        private Scrollbar typeScrollBar;

        private float NormalizedMoveSpeed(float moveSpeed)
        {
            return Mathf.Clamp01(Mathf.InverseLerp(1f, 5f, moveSpeed));
        }

        private float NormalizedTypeSpeed(float typeSpeed)
        {
            typeSpeed = 0.08f * 2 - typeSpeed;
            return Mathf.Clamp01(Mathf.InverseLerp(0.05f, 0.12f, typeSpeed));
        }

        private float NormalizedSprintMultiplier(float sprintMultiplier)
        {
            return Mathf.Clamp01(Mathf.InverseLerp(2f, 5f, sprintMultiplier));
        }

        private float UnnormalizedMoveSpeed(float normalized)
        {
            return Mathf.Lerp(1f, 5f, Mathf.Clamp01(normalized));
        }

        private float UnnormalizedTypeSpeed(float normalized)
        {
            return Mathf.Lerp(0.05f, 0.12f, Mathf.Clamp01(normalized));
        }

        private float UnnormalizedSprintMultiplier(float normalized)
        {
            return Mathf.Lerp(2f, 5f, Mathf.Clamp01(normalized));
        }

        private void OnEnable()
        {
            SetScrollBar();
        }

        private void SetScrollBar()
        {
            float moveSpeed = SettingsMgr.Instance.GetPlayerSpeed();
            moveScrollBar.SetValueWithoutNotify(NormalizedMoveSpeed(moveSpeed));
            float sprintMultiplier = SettingsMgr.Instance.GetSprintMultiplier();
            sprintScrollBar.SetValueWithoutNotify(NormalizedSprintMultiplier(sprintMultiplier));
            float typeSpeed = SettingsMgr.Instance.GetTypeSpeed();
            typeScrollBar.SetValueWithoutNotify(NormalizedTypeSpeed(typeSpeed));
        }

        public void HandleResetDefaults()
        {
            var fields = new[]
            {
                SettingField.PlayerSpeed,
                SettingField.TypeSpeed,
                SettingField.SprintMultiplier,
            };

            SettingsMgr.Instance.ResetToDefaults(fields);
            SetScrollBar();
        }

        public void HandleMoveSpeedChange()
        {
            // 当速度滑条数值改变时，把最新值写回SettingsMgr
            float value = moveScrollBar.value;
            SettingsMgr.Instance.SetPlayerSpeed(UnnormalizedMoveSpeed(value));
        }

        public void HandleTypeSpeedChange()
        {
            // 当打字速度滑条数值改变时，把最新值写回SettingsMgr
            float value = typeScrollBar.value;
            value = 2 * 0.08f - value;
            SettingsMgr.Instance.SetTypeSpeed(UnnormalizedTypeSpeed(value));
        }

        public void HandleSprintMultiplierChange()
        {
            // 当疾跑滑条数值改变时，把最新值写回 AudioManager
            float value = sprintScrollBar.value;
            SettingsMgr.Instance.SetSprintMultiplier(UnnormalizedSprintMultiplier(value));
        }
    }
}
