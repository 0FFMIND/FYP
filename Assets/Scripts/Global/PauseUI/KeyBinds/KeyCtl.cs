// MVC/Controller/KeyRebindCtl.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Manager;      // SettingsMgr, InputAction
using Utils;        // EventBus, EKeyRebindRequest

namespace MVC
{
    public class KeyRebindCtl : MonoBehaviour
    {
        [Header("列表根")]
        [SerializeField] private Transform content;     // 含有若干 KeyView 的容器
        [Header("Modal")]
        [SerializeField] private GameObject modalRoot;  // 半透明遮罩+面板
        [SerializeField] private TMP_Text modalTitle;   // 可选标题

        private readonly Dictionary<InputAction, KeyView> _views = new();
        private Coroutine _listen;

        private void OnEnable()
        {
            BuildIndex();
            EventBus.Subscribe<EKeyRebindRequest>(OnRebindRequest);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EKeyRebindRequest>(OnRebindRequest);
            if (_listen != null) StopCoroutine(_listen);
            HideModal();
        }

        private void BuildIndex()
        {
            _views.Clear();
            foreach (var v in content.GetComponentsInChildren<KeyView>(true))
            {
                // 这里假设 KeyView.Bind 已经被调用过，Action 有值
                if (!_views.ContainsKey(v.Action))
                    _views.Add(v.Action, v);
            }
        }

        private void OnRebindRequest(EKeyRebindRequest e)
        {
            // 打开弹窗并开始监听一次键入
            ShowModal($"为 {Pretty(e.Action)} 选择按键（Esc 取消）");
            if (_listen != null) StopCoroutine(_listen);
            _listen = StartCoroutine(CaptureOnce(e.Action));
        }

        private IEnumerator CaptureOnce(InputAction action)
        {
            yield return null; // 防抖：跳过点击按钮这一帧

            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Escape)) break;

                foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
                {
                    // 如不想允许鼠标键，可保留这句
                    if (code >= KeyCode.Mouse0 && code <= KeyCode.Mouse6) continue;

                    if (Input.GetKeyDown(code))
                    {
                        ApplyRebind(action, code);
                        if (_views.TryGetValue(action, out var v))
                            v.SetKeyText(code);       // 刷新该行显示
                        goto END;
                    }
                }
                yield return null;
            }
        END:
            HideModal();
            _listen = null;
        }

        private void ApplyRebind(InputAction action, KeyCode newKey)
        {
            var sm = SettingsMgr.Instance;
            var map = sm.Current.keyBindings;

            // 冲突处理：把占用 newKey 的其他动作清成 None（你也可改成“交换/提示确认”）
            foreach (var k in map.Keys.ToList())
                if (!k.Equals(action) && map[k] == newKey)
                    map[k] = KeyCode.None;

            map[action] = newKey;

            sm.Save();
            EventBus.Publish(new ESettingsChanged(sm.Current)); // 通知 InputManager 等同步
        }

        private void ShowModal(string title)
        {
            if (modalRoot) modalRoot.SetActive(true);
            if (modalTitle) modalTitle.text = title;
        }

        private void HideModal()
        {
            if (modalRoot) modalRoot.SetActive(false);
        }

        private string Pretty(InputAction a) => a switch
        {
            InputAction.DialogueClick => "对话推进",
            InputAction.PlayerSprint => "玩家冲刺",
            InputAction.PauseGame => "暂停/返回",
            _ => a.ToString()
        };
    }
}
