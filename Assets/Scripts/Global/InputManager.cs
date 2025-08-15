using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.SingletonPattern;

namespace InputSystem
{
    public enum InputAction
    {
        DialogueClick,
        PlayerSprint,
    }

    public class InputManager : SingletonMB<InputManager>
    {
        // PC°´¼üÓ³Éä
        public List<PCMapping> defaultMappings = new List<PCMapping>
        {
            new PCMapping
            {
                action = InputAction.DialogueClick,
                keys = new KeyCode[] { KeyCode.Return },
            },
            new PCMapping
            {
                action = InputAction.PlayerSprint,
                keys = new KeyCode[] { KeyCode.LeftShift },
            },
        };
        public event Action<InputAction> OnAction;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnAction?.Invoke(InputAction.DialogueClick);
            }
            foreach (var map in defaultMappings)
            {
                foreach (var key in map.keys)
                {
                    if (Input.GetKeyDown(key))
                    {
                        OnAction?.Invoke(map.action);
                        break;
                    }
                }
            }
        }
    }

    [Serializable]
    public struct PCMapping
    {
        public InputAction action;
        public KeyCode[] keys;
    }
}
