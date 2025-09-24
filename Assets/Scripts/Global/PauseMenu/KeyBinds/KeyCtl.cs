using System;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;
using Utils;

namespace MVC
{
    public class KeyCtl : MonoBehaviour
    {
        // KeyView 列表的根节点
        [SerializeField]
        private Transform viewRoot;

        // KeyView 预制体
        [SerializeField]
        private KeyView keyViewPrefab;

        // 弹窗
        [SerializeField]
        private ModalCtl modal;

        // 映射表：InputAction → KeyView
        private readonly Dictionary<InputAction, KeyView> _views = new();

        // 记录当前已应用到 UI 上的键位（用于快速比较）
        private readonly Dictionary<InputAction, KeyCode> _appliedBindings = new();

        // 禁止修改方向键
        private readonly HashSet<KeyCode> _disallowedRebindKeys = new()
        {
            KeyCode.W,
            KeyCode.A,
            KeyCode.S,
            KeyCode.D,
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
        };

        private bool IsDisallowedRebindKey(KeyCode key) => _disallowedRebindKeys.Contains(key);

        private void Awake()
        {
            // 第一次创建一次所有行
            EnsureDefaultViews();
        }

        private void OnEnable()
        {
            // 填充列表（从 SettingsMgr 读取）
            RefreshAllViewsFromSettings();
        }

        private void EnsureDefaultViews()
        {
            if (viewRoot == null || keyViewPrefab == null)
            {
                Debug.LogError("[KeyCtl] 请在 Inspector 绑定 viewRoot 与 keyViewPrefab");
                return;
            }
            // 先将父节点设为未激活，避免 LocalizedText 因未设 key 而报错/被禁用
            bool wasActive = viewRoot.gameObject.activeSelf;
            viewRoot.gameObject.SetActive(false);

            // 按枚举顺序创建所有行
            foreach (InputAction action in Enum.GetValues(typeof(InputAction)))
            {
                var go = Instantiate(keyViewPrefab.gameObject, viewRoot);
                var kv = go.GetComponent<KeyView>();
                // 临时绑定：显示名和占位 key（会在 RefreshAllViewsFromSettings 中覆盖）
                kv.Bind(action, Pretty(action), KeyCode.None);

                // 缓存
                _views[action] = kv;

                // 订阅该key的重绑请求
                Action<InputAction> handler = OnKeyViewRequestRebind;
                kv.OnRequestRebind += handler;
            }
            // 统一激活；这时 key 已就绪，刷新无警告
            viewRoot.gameObject.SetActive(wasActive);
        }

        private void RefreshAllViewsFromSettings()
        {
            Dictionary<InputAction, KeyCode> settings = SettingsMgr.Instance.GetKeyBindings();

            // 用枚举顺序遍历所有已缓存行
            foreach (InputAction action in Enum.GetValues(typeof(InputAction)))
            {
                // 首先确保我们有对应的 KeyView（正常情况下 EnsureDefaultViews 已创建）
                if (!_views.TryGetValue(action, out var view))
                {
                    Debug.LogWarning($"[KeyCtl] 未找到对应的 KeyView（{action}），跳过刷新");
                    continue;
                }

                // 从 settings 取应当显示的键；若没有则视为 None
                KeyCode shouldKey = settings.TryGetValue(action, out var k) ? k : KeyCode.None;

                // 取出当前已应用到 UI 的键，如果没记录则默认 KeyCode.None
                _appliedBindings.TryGetValue(action, out var appliedKey);

                // 若相同则跳过（避免重复更新）
                if (appliedKey == shouldKey)
                    continue;

                // 否则更新
                view.SetKeyCode(shouldKey);

                // 更新缓存记录
                _appliedBindings[action] = shouldKey;
            }
        }

        private void OnKeyViewRequestRebind(InputAction action)
        {
            // 取当前键
            KeyCode currentKey = _appliedBindings[action];
            // 取当前文本
            string keyText = Pretty(action);
            // 打开弹窗并开始监听一次键入
            ShowModal(action, keyText, currentKey, ApplyRebind);
        }

        private void ApplyRebind(InputAction action, KeyCode newKey)
        {
            if (newKey == _appliedBindings[action])
            {
                // 新按键与旧按键相同
                HideModal();
                return;
            }
            if (IsDisallowedRebindKey(newKey))
            {
                // 禁止修改方向键
                HideModal();
                return;
            }
            // 用枚举顺序遍历所有已缓存行
            foreach (InputAction _action in Enum.GetValues(typeof(InputAction)))
            {
                if (_action == action)
                {
                    continue;
                }
                // 按键冲突
                if (_appliedBindings[_action] == newKey)
                {
                    HideModal();
                    return;
                }
            }
            // 通知更改
            EventBus.Publish(new EKeySet(action, newKey));
            // UI更新
            _appliedBindings[action] = newKey;
            _views[action].SetKeyCode(newKey);
            // 关闭modal
            HideModal();
        }

        private void ShowModal(
            InputAction action,
            string keyText,
            KeyCode key,
            Action<InputAction, KeyCode> onConfirm
        )
        {
            // 暂时屏蔽全局 Pause 响应，避免 ESC 触发 PauseMgr
            PauseMgr.Instance.SetPauseEnabled(false);

            modal.OpenAuthorized(action, keyText, key, onConfirm);
        }

        private void HideModal()
        {
            // 恢复全局 Pause 响应
            PauseMgr.Instance.SetPauseEnabled(true);

            modal.Close();
        }

        private string Pretty(InputAction a) =>
            a switch
            {
                InputAction.DialogueClick => "对话推进",
                InputAction.PlayerSprint => "玩家疾跑",
                InputAction.PauseGame => "暂停/返回",
                _ => a.ToString(),
            };
    }
}
