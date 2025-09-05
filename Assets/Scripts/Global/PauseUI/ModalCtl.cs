using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;

namespace MVC
{
    public class ModalCtl : MonoBehaviour
    {
        // 仅当 KeyCtl 授权时允许显示
        private bool _authorized = false;

        // 当前正在重绑的动作
        private InputAction _action;

        [SerializeField]
        private TMP_Text tipText;

        // 捕获到新键后的回调（由 KeyCtl 提供）
        private Action<InputAction, KeyCode> _onConfirm;

        private void Update()
        {
            // 未授权时不工作（防止被父物体误激活触发）
            if (!_authorized)
            {
                return;
            }
            // 侦测本帧按下的第一个键
            var kc = DetectKeyDown();
            // 没有有效按键关闭
            if (kc == KeyCode.None)
            {
                return;
            }
            _onConfirm?.Invoke(_action, kc);
        }

        private KeyCode DetectKeyDown()
        {
            // 枚举 KeyCode，找第一个 GetKeyDown 的键
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                if (k == KeyCode.None)
                    continue;
                // 过滤鼠标
                if (k >= KeyCode.Mouse0 && k <= KeyCode.Mouse6)
                    continue;
                if (Input.GetKeyDown(k))
                    return k;
            }
            return KeyCode.None;
        }

        public void OpenAuthorized(
            InputAction action,
            string keyText,
            KeyCode key,
            Action<InputAction, KeyCode> onConfirm
        )
        {
            // 记录当前动作
            _action = action;
            // 刷新提示文案
            SetTip(keyText, key);
            // 注册确认回调
            _onConfirm = onConfirm;
            _authorized = true;
            gameObject.SetActive(true);
        }

        private void SetTip(string keyText, KeyCode key)
        {
            tipText
                .GetComponent<LocalizedText>()
                .SetKey(
                    $"正在为{keyText}重新绑定，当前按键：{key.ToString()}\n请按任意键设置新按键\n提示：如果 {key.ToString()} 已被其他操作使用，此次更改将不会应用"
                );
        }

        public void Close()
        {
            _authorized = false;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // 防止 ShowPage/父节点激活误开
            if (!_authorized)
                gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            // 退出时清除授权，避免下次被动激活
            _authorized = false;
        }
    }
}
