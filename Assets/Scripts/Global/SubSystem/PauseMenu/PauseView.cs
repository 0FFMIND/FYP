using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public class PauseView : MonoBehaviour
    {
        [Serializable]
        public class PageTab
        {
            public PauseMenuToggleView toggleView;
            public GameObject page; // 对应页面根节点
        }

        // 互斥分组
        [SerializeField]
        private ToggleGroup toggleGroup;

        [Header("Tabs")]
        [SerializeField]
        private List<PageTab> tabs = new List<PageTab>();
        private readonly Dictionary<PageTab, int> _indexOf = new Dictionary<PageTab, int>();

        public Scene1UIGuideCtl guidectl;
        public bool showGuide = false;

        // 折叠动画时长
        [Min(0.0001f)]
        [SerializeField]
        private float foldDuration;

        [SerializeField]
        private GameObject pauseMenuRoot;

        [SerializeField]
        private Animator animator;
        private int _mainIndex = 0;

        // 是否正在进行动画
        private Coroutine _transitionCo;
        public bool IsTransitioning { get; private set; }

        private void Awake()
        {
            if (pauseMenuRoot == null)
            {
                pauseMenuRoot = gameObject;
            }
            _indexOf.Clear();
            for (int i = 0; i < tabs.Count; i++)
            {
                var t = tabs[i];
                _indexOf[t] = i;

                // 绑定按钮：点击→ShowPage(i, withFold:true)
                if (t.toggleView)
                {
                    int captured = i;
                    t.toggleView.Bind(toggleGroup);

                    // 订阅选中事件
                    t.toggleView.OnSelected += () =>
                    {
                        AudioMgr.Instance.PlaySFX("buttonClick");
                        ShowPage(captured);
                        _mainIndex = captured;
                    };
                }

                // 入场前先都隐藏
                if (t.page)
                    t.page.SetActive(false);
            }
        }

        private void OnEnable()
        {
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        public void Show()
        {
            if (!pauseMenuRoot)
            {
                Debug.LogError("[PauseView] Show() 失败：pauseMenuRoot 为 null。", this);
                return;
            }
            // 若有未完成的过渡，先停掉再开始（避免并发协程）
            if (_transitionCo != null)
            {
                StopCoroutine(_transitionCo);
                _transitionCo = null;
            }
            pauseMenuRoot.SetActive(true);
            ShowPage(-1);
            // 播放进场动画
            IsTransitioning = true;
            _transitionCo = StartCoroutine(EnterPauseMenu("PauseMenuEnter", 0.2f));
        }

        public void Hide(Action callback)
        {
            if (_transitionCo != null)
            {
                StopCoroutine(_transitionCo);
                _transitionCo = null;
            }
            IsTransitioning = true;
            _transitionCo = StartCoroutine(ExitPauseMenu("PauseMenuExit", 0.2f, callback));
        }

        public void ShowPage(int i)
        {
            // 只显示第 i 页，其他页全部隐藏
            for (int k = 0; k < tabs.Count; k++)
            {
                var page = tabs[k].page;
                if (page)
                {
                    page.SetActive(i >= 0 && k == i);
                }
            }
        }

        // 退场动画
        private IEnumerator ExitPauseMenu(string stateName, float waitSeconds, Action callback)
        {
            yield return FoldYOut(tabs[_mainIndex].page.GetComponent<RectTransform>());
            yield return new WaitForSecondsRealtime(0.1f);
            yield return PlayAnimState(stateName, waitSeconds);

            callback?.Invoke();
            IsTransitioning = false;
            _transitionCo = null;
        }

        // 入场动画
        private IEnumerator EnterPauseMenu(string stateName, float waitSeconds)
        {
            AudioMgr.Instance.PlaySFX("menuOpen");
            yield return PlayAnimState(stateName, waitSeconds);
            yield return new WaitForSecondsRealtime(0.1f);
            ShowPage(_mainIndex);
            yield return FoldYIn(tabs[_mainIndex].page.GetComponent<RectTransform>());
            IsTransitioning = false;
            _transitionCo = null;
            if (showGuide)
            {
                PauseMgr.Instance.canPause = false;
                guidectl.StartShowSequence(() =>
                {
                    PauseMgr.Instance.canPause = true;
                    showGuide = false;
                });
            }
        }

        private IEnumerator FoldYIn(RectTransform rt)
        {
            // 0 -> 1
            yield return FoldY(rt, 0f, 1f, foldDuration);
        }

        private IEnumerator FoldYOut(RectTransform rt)
        {
            // 1 -> 0
            yield return FoldY(rt, 1f, 0f, foldDuration);
        }

        private IEnumerator FoldY(RectTransform rt, float from, float to, float duration)
        {
            if (!rt)
                yield break;
            float dur = Mathf.Max(0.0001f, duration);
            AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            // 只改y，保留原本的 x/z 比例
            float x = rt.localScale.x;
            float z = rt.localScale.z;

            // 入场前确保可见
            if (!rt.gameObject.activeSelf)
                rt.gameObject.SetActive(true);
            rt.localScale = new Vector3(x, from, z);

            float t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime; // 不受暂停影响
                float u = Mathf.Clamp01(t / dur);
                float w = curve.Evaluate(u);
                float y = Mathf.LerpUnclamped(from, to, w);
                rt.localScale = new Vector3(x, y, z);
                yield return null;
            }
            rt.localScale = new Vector3(x, to, z);
        }

        private IEnumerator PlayAnimState(string stateName, float waitSeconds)
        {
            animator.Play(stateName, 0, 0f);
            yield return new WaitForSecondsRealtime(waitSeconds);
        }
    }
}
