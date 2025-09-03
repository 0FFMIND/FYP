using System;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public class KeyView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text actionName;

        [SerializeField]
        private Button keyButton;

        [SerializeField]
        private TMP_Text keyText;

        private InputAction _action;
        public event Action<InputAction> OnRequestRebind;

        public void Bind(InputAction action, string displayName, KeyCode key)
        {
            _action = action;
            if (actionName)
            {
                actionName.text = displayName;
            } // 将 KeyCode 转为文本并显示
            SetKeyText(key);

            // 清除按钮上已有的监听，避免重复添加
            keyButton.onClick.RemoveAllListeners();
            keyButton.onClick.AddListener(() => OnRequestRebind?.Invoke(_action));
        }

        public void SetKeyText(KeyCode key)
        {
            keyText.text = key.ToString();
        }
    }
}
