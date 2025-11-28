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

        protected int visitCount = 0; // 已访问次数
        private InteractModel _curStep; // 当前这一次使用的步骤

        protected PlayerCtl _player;

        [Header("标记")]
        [SerializeField]
        public bool isImportant = false; // 重要交互（主线/关键）

        [NonSerialized]
        public bool isTalked = false; // 是否已完整聊过（运行时置位）

        [SerializeField]
        protected InteractDialogCtl interactCtl;

        [SerializeField]
        protected TimelineDialogCtl dialogCtl;

        // 是否处于对话中
        private bool isInteracting;

        private bool shouldEndInteract;
        // 选项
        [SerializeField]
        private ChoiceCtl choiceCtl;

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

        protected void BeginDialogue()
        {
            _curStep = StepFor(visitCount);

            if (_curStep == null)
            {
                Debug.LogWarning("[InteractCtl] BeginInteract -> FALSE (no step/lines).");
                return;
            }
            // 判定各段是否存在
            bool hasFirst = _curStep.firstLines != null && _curStep.firstLines.Length > 0;
            bool hasSecondLines = _curStep.secondLines != null && _curStep.secondLines.Length > 0;
            bool hasSecondMaps = _curStep.secondMappings != null && _curStep.secondMappings.Length > 0;
            bool hasChoice = _curStep.choiceModel.items != null && _curStep.choiceModel.items.Length > 0;
            bool endThisTime = shouldEndInteract;

            // 启动 second 段（走 dialogCtl），容错：映射或台词缺失则传空数组
            void StartSecondFlow()
            {
                if (hasChoice)
                {
                    dialogCtl.hasChoice = true;
                    dialogCtl.choiceModel = _curStep.choiceModel;
                }
                else
                {
                    dialogCtl.hasChoice = false;
                    dialogCtl.choiceModel.items = null;
                }

                var maps = hasSecondMaps ? _curStep.secondMappings : Array.Empty<LineMapping>();
                var lines = hasSecondLines ? _curStep.secondLines : Array.Empty<string>();

                dialogCtl.StartInteractDialogue(
                    maps,
                    lines,
                    () =>
                    {
                        if (endThisTime)
                            EndInteract(_player);
                    }
                );
            }

            // 情况A：first 不存在而 second 存在 → 直接进入 second（不走 interactCtl）
            if (!hasFirst && hasSecondLines)
            {
                StartSecondFlow();
                return;
            }

            // 情况B：first 存在 → 先走交互对话，回调里再决定是否进 second
            if (hasFirst)
            {
                interactCtl.StartDialogue(
                    _curStep.firstLines,
                    () =>
                    {
                        if (hasSecondLines)
                        {
                            StartSecondFlow();
                        }
                        else
                        {
                            if (endThisTime)
                                EndInteract(_player);
                        }
                    }
                );
            }
            else
            {
                if (endThisTime)
                    EndInteract(_player);
            }
            shouldEndInteract = true; // 重置默认值
        }

        public virtual bool BeginInteract(PlayerCtl player, bool shouldEndInteract = true)
        {
            this.shouldEndInteract = shouldEndInteract;
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
            // 持有玩家引用
            _player = player;
            // 进入交互状态
            isInteracting = true;
            // 启动对话
            BeginDialogue();
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
