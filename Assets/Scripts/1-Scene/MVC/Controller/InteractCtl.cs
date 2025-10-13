using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace MVC
{
    public class InteractCtl : MonoBehaviour, IInteractable
    {
        [Header("每次交互配置（第 i 次）")]
        [SerializeField]
        private InteractModel[] steps;

        private int visitCount = 0; // 已访问次数
        private InteractModel _curStep; // 当前这一次使用的步骤

        private PlayerCtl _player;

        [Header("标记")]
        [SerializeField]
        public bool isImportant = false; // 重要交互（主线/关键）

        [NonSerialized]
        public bool isTalked = false; // 是否已完整聊过（运行时置位）

        [SerializeField]
        private InteractDialogCtl interactCtl;

        [SerializeField]
        private TimelineDialogCtl dialogCtl;

        // 是否处于对话中
        private bool isInteracting;

        // 提供只读访问，供 PlayerInteractCtl 判定
        public bool IsImportant => isImportant;
        public bool IsTalked => isTalked;

        // 根据当前访问次数 vc，挑选“起始阈值 visit ≤ vc 且最大”的步骤
        private InteractModel StepFor(int vc)
        {
            // 若未配置 steps，直接返回 null
            if (steps == null || steps.Length == 0)
                return null;
            InteractModel best = null;
            int bestStart = int.MinValue;
            for (int i = 0; i < steps.Length; i++)
            {
                var s = steps[i];
                if (s == null)
                    continue;
                // 若该步骤的阈值 ≤ vc 且比已选的阈值更大
                if (vc >= s.visit && s.visit > bestStart)
                {
                    best = s;
                    bestStart = s.visit;
                }
            }
            // 没命中就用第一个（可按需改成 null）
            return best ?? steps[0];
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

            _curStep = StepFor(visitCount);
            if (_curStep == null || _curStep.lines == null || _curStep.lines.Length == 0)
            {
                Debug.LogWarning("[InteractCtl] BeginInteract -> FALSE (no step/lines).");
                return false;
            }
            _player = player;

            isInteracting = true;

            bool hasMapping = _curStep.mappings != null && _curStep.mappings.Length > 0;

            // 先走交互对话（带 linemapping）
            interactCtl.StartDialogue(
                _curStep.lines,
                () =>
                {
                    // 回调
                    if (hasMapping)
                    {
                        dialogCtl.StartInteractDialogue(
                            _curStep.mappings,
                            _curStep.secondLines,
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
            // 次数+1
            visitCount++;
            // 更新标志位
            isTalked = true;
            _curStep.onInteractEnd?.Invoke();
            // 广播结束
            EventBus.Publish(new EInteractEnd());

            _player = null;
        }
    }
}
