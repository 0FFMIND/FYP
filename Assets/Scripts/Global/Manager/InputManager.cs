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
        // PC按键映射
        public List<PCMapping> defaultMappings = new List<PCMapping>
        {
            new PCMapping { action = InputAction.DialogueClick, key = KeyCode.Return },
            new PCMapping { action = InputAction.PlayerSprint, key = KeyCode.LeftShift },
            new PCMapping { action = InputAction.PauseGame, key = KeyCode.Escape },
        };
        public event Action<InputAction> OnAction;
        private Dictionary<InputAction, Action> actionEvents = new();

        public void Subscribe(InputAction action, Action callback)
        {
            if (!actionEvents.ContainsKey(action))
                actionEvents[action] = () => { };
            actionEvents[action] += callback;
        }

        public void Unsubscribe(InputAction action, Action callback)
        {
            if (actionEvents.ContainsKey(action))
                actionEvents[action] -= callback;
        }

        private void Update()
        {
            // 花括号避免handler同作用域
            {
                if (
                    Input.GetMouseButtonDown(0)
                    && actionEvents.TryGetValue(InputAction.DialogueClick, out var handler)
                )
                {
                    handler?.Invoke();
                }
            }
            foreach (var map in defaultMappings)
            {
                if (
                    Input.GetKeyDown(map.key)
                    && actionEvents.TryGetValue(map.action, out var handler)
                )
                {
                    handler?.Invoke();
                    break;
                }
            }
        }
    }
}
