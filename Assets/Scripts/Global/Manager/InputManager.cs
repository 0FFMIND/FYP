using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Manager
{
    [Serializable]
    public enum InputAction
    {
        DialogueClick,
        PlayerSprint,
        PauseGame,
    }

    [Serializable]
    public struct PCMapping
    {
        public InputAction action;
        public KeyCode key;
    }

    public class InputManager : SingletonMB<InputManager>
    {
        private readonly Dictionary<InputAction, KeyCode> _bindings = new();

        // AutoSingletonMB
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            EnsureCreated();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ESettingsChanged>(OnSettingsChanged);
            EventBus.Subscribe<EKeyRebind>(OnBindingsChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ESettingsChanged>(OnSettingsChanged);
            EventBus.Unsubscribe<EKeyRebind>(OnBindingsChanged);

        }

        private void OnBindingsChanged(EKeyRebind e)
        {
            _bindings[e.Action] = e.Key;
        }


        // 当 SettingsMgr 广播 Settings 改变时会调用此方法（e.Settings 包含新的 keyBindings）
        private void OnSettingsChanged(ESettingsChanged e)
        {
            var newMap = e.Settings.keyBindings;
            if (newMap == null)
                return;
            ApplyBindings(newMap);
        }

        private void ApplyBindings(Dictionary<InputAction, KeyCode> newMap)
        {
            if (newMap == null)
                return;

            // 如果与当前 _bindings 完全一致，则跳过
            if (MappingsEqual(_bindings, newMap))
                return;

            // 更新内部字典
            _bindings.Clear();
            foreach (var kv in newMap)
                _bindings[kv.Key] = kv.Value;
        }

        // 比较两个字典是否相同
        private bool MappingsEqual(
            Dictionary<InputAction, KeyCode> a,
            Dictionary<InputAction, KeyCode> b
        )
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null)
                return false;
            if (a.Count != b.Count)
                return false;
            foreach (var kv in a)
            {
                if (!b.TryGetValue(kv.Key, out var v) || v != kv.Value)
                    return false;
            }
            return true;
        }

        private void Update()
        {
            // 特例：鼠标左键当作对话推进（按需保留）
            if (Input.GetMouseButtonDown(0))
            {
                EventBus.Publish(
                    InputAction.DialogueClick,
                    new EInputPressed(InputAction.DialogueClick)
                );
                return;
            }

            foreach (var kv in _bindings)
            {
                var action = kv.Key;
                var key = kv.Value;
                if (key == KeyCode.None)
                    continue;
                if (Input.GetKeyDown(key))
                {
                    EventBus.Publish(action, new EInputPressed(action));
                    break;
                }
            }
        }
    }
}
