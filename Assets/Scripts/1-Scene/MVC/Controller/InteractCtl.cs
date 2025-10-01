using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class InteractCtl : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private InteractModel _model;
        private PlayerCtl _player;

        // 是否处于对话中
        private bool isInteracting;

        // 提供只读访问，供 PlayerInteractCtl 判定
        public bool IsImportant => _model != null && _model.isImportant;
        public bool IsTalked => _model != null && _model.isTalked;

        private void OnEnable()
        {
            EventBus.Subscribe<EInputPressed, InputAction>(
                InputAction.DialogueClick,
                OnInteractPressed
            );
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EInputPressed, InputAction>(
                InputAction.DialogueClick,
                OnInteractPressed
            );
        }

        public bool BeginInteract(PlayerCtl player)
        {
            if (isInteracting)
            {
                Debug.Log($"[InteractCtl] BeginInteract -> FALSE (already interacting).");
                return false;
            }
            if (player == null)
            {
                Debug.LogWarning($"[InteractCtl] BeginInteract -> FALSE (player is null).");
                return false;
            }
            if (_model == null)
            {
                Debug.LogWarning($"[InteractCtl] BeginInteract -> FALSE (_model is null).");
                return false;
            }
            _player = player;
            // 重置交互模型（回到第一行）
            _model.Reset();

            isInteracting = true;

            // 广播交互事件，让 InteractView 显示第一行
            EventBus.Publish(new EInteract(_model));
            return true;
        }

        public void EndInteract(PlayerCtl player)
        {
            if (!isInteracting)
            {
                return;
            }
            isInteracting = false;
            // 更新标志位
            if (_model != null)
            {
                _model.isTalked = true;
            }
            // 广播结束
            EventBus.Publish(new EInteractEnd());

            _player = null;
        }

        private void OnInteractPressed()
        {
            if (!isInteracting || _model == null)
                return;

            // 推进一行；没有内容则结束
            if (_model.Next())
            {
                EventBus.Publish(new EInteract(_model)); // 让 View 刷新
            }
            else
            {
                EndInteract(_player);
            }
        }
    }
}
