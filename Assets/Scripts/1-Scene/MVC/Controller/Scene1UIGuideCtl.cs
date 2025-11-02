using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    [Serializable]
    public class GuideStep
    {
        public RectTransform panel; // 该步自己的说明/提示 Panel
        public Toggle nextToggle; // 该步的“下一步”按钮（可为空）
    }

    public class Scene1UIGuideCtl : MonoBehaviour
    {
        [Header("步骤（每步的面板+按钮）")]
        [SerializeField]
        private List<GuideStep> _steps;

        [SerializeField]
        private Transform panelRoot;
        private int _idx;
        private Action _onDone;

        [Header("全局根物体（用于 idx==1 的硬编码查找）")]
        [SerializeField]
        private GameObject guideRoot;

        private void Awake()
        {
            if (panelRoot)
                panelRoot.gameObject.SetActive(false);
            // 确保 Inspector 里其它 panel 默认关闭
            if (_steps != null)
            {
                foreach (var s in _steps)
                {
                    if (s?.panel)
                        s.panel.gameObject.SetActive(false);
                }
            }
        }

        private void OnDisable()
        {
            // 清理当前步的监听，避免对象失活后堆积
            UnhookToggle(_idx);
        }

        // 从外部传入步骤
        public void StartShowSequence(Action onDone)
        {
            _idx = 0;
            _onDone = onDone;

            panelRoot.gameObject.SetActive(true);
            ShowStep(_idx);
        }

        public void Next()
        {
            if (_steps == null)
                return;

            // 收尾当前步
            UnhookToggle(_idx);
            SetPanelActive(_idx, false);

            _idx++;
            if (_idx >= _steps.Count)
            {
                // 结束
                panelRoot.gameObject.SetActive(false);
                _onDone?.Invoke();
                return;
            }

            ShowStep(_idx);
        }

        private void ShowStep(int idx)
        {
            if (_steps == null || idx < 0 || idx >= _steps.Count)
                return;

            // 仅该步 panel 激活
            SetPanelActive(idx, true);

            // 绑定该步按钮推进
            HookToggle(idx);
        }

        private void SetPanelActive(int idx, bool active)
        {
            if (_steps == null || idx < 0 || idx >= _steps.Count)
                return;
            for (int i = 0; i < _steps.Count; i++)
            {
                var p = _steps[i]?.panel;
                if (p)
                    p.gameObject.SetActive(active && i == idx);
            }
        }

        private void Update()
        {
            if (_idx == 1)
            {
                // 递归找到名为 "JournalTitle_exploreRooftop" 的 Transform
                Transform target = null;
                foreach (var t in guideRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name == "JournalTitle_exploreRooftop")
                    {
                        target = t;
                        break;
                    }
                }
                if (!target)
                    return;

                // 在该物体下拿一个 Toggle（自己或子物体）
                _steps[_idx].nextToggle = target.GetComponentInChildren<Toggle>(true);
                // 进入该步先重置状态，避免上一步遗留为 true 直接越过
                _steps[_idx].nextToggle.onValueChanged.RemoveListener(OnToggleChanged);
                _steps[_idx].nextToggle.isOn = false;
                _steps[_idx].nextToggle.onValueChanged.AddListener(OnToggleChanged);
                _steps[_idx].nextToggle.interactable = true;
            }
        }

        private void HookToggle(int idx)
        {
            if (idx == 1)
            {
                // 递归找到名为 "JournalTitle_exploreRooftop" 的 Transform
                Transform target = null;
                foreach (var t in guideRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name == "JournalTitle_exploreRooftop")
                    {
                        target = t;
                        break;
                    }
                }
                if (!target)
                    return;

                // 在该物体下拿一个 Toggle（自己或子物体）
                _steps[idx].nextToggle = target.GetComponentInChildren<Toggle>(true);
            }
            if (!InRange(idx))
                return;
            var tgl = _steps[idx]?.nextToggle;
            if (!tgl)
                return;
            // 进入该步先重置状态，避免上一步遗留为 true 直接越过
            tgl.onValueChanged.RemoveListener(OnToggleChanged);
            tgl.isOn = false;
            tgl.onValueChanged.AddListener(OnToggleChanged);
            tgl.interactable = true;
        }

        private void OnToggleChanged(bool isOn)
        {
            if (isOn)
                Next();
        }

        private void UnhookToggle(int idx)
        {
            if (!InRange(idx))
                return;
            var tgl = _steps[idx]?.nextToggle;
            if (!tgl)
                return;
            tgl.onValueChanged.RemoveListener(OnToggleChanged);
        }

        private bool InRange(int idx)
        {
            return _steps != null && idx >= 0 && idx < _steps.Count;
        }
    }
}
