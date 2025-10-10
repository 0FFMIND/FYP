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

        [SerializeField]
        private InteractDialogCtl interactCtl;

        [SerializeField]
        private TimelineDialogCtl dialogCtl;

        // 是否处于对话中
        private bool isInteracting;

        // 提供只读访问，供 PlayerInteractCtl 判定
        public bool IsImportant => _model != null && _model.isImportant;
        public bool IsTalked => _model != null && _model.isTalked;

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

            isInteracting = true;

            bool hasMapping = _model.mappings != null && _model.mappings.Length > 0;

            // 先走交互对话（带 linemapping）
            interactCtl.StartDialogue(
                _model.lines,
                () =>
                {
                    // 回调
                    if (hasMapping)
                    {
                        dialogCtl.StartInteractDialogue(
                            _model.mappings,
                            _model.secondLines,
                            () =>
                            {
                                EndInteract(_player);
                            }
                        );
                    }
                    else
                    {
                        EndInteract(_player);
                    }
                }
            );
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
    }
}
