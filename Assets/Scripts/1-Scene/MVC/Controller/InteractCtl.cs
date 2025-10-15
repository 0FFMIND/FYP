using System;
using UnityEngine;
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
            if (_curStep == null || _curStep.firstLines == null || _curStep.firstLines.Length == 0)
            {
                Debug.LogWarning("[InteractCtl] BeginInteract -> FALSE (no step/lines).");
                return false;
            }
            _player = player;

            isInteracting = true;

            bool hasMapping = _curStep.secondMappings != null && _curStep.secondMappings.Length > 0;

            // 先走交互对话（带 linemapping）
            interactCtl.StartDialogue(
                _curStep.firstLines,
                () =>
                {
                    // 回调
                    if (hasMapping)
                    {
                        dialogCtl.StartInteractDialogue(
                            _curStep.secondMappings,
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
            // 有持久监听（Inspector 上绑定了方法）→ 传 this，让监听方在合适时机调用 ctl.Done()
            bool hasListeners =
                _curStep != null
                && _curStep.onInteractEnd != null
                && _curStep.onInteractEnd.GetPersistentEventCount() > 0;

            if (hasListeners)
            {
                _curStep.onInteractEnd.Invoke(this); // 动态参数：当前 InteractCtl
            }
            else
            {
                Done(); // 没绑定就立刻收尾
            }
        }

        // 提供给监听方在流程末尾调用
        public void Done()
        {
            // 退出交互状态
            isInteracting = false;
            // 访问次数 +1
            visitCount++;
            // 本对象标记为已聊完
            isTalked = true;
            // 广播结束事件（PlayerInteractCtl 等会解除吞键/高亮等）
            EventBus.Publish(new EInteractEnd());
            // 释放玩家引用
            _player = null;
        }
    }
}
