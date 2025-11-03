using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace Manager
{
    public class TransitionMgr : SingletonMB<TransitionMgr>
    {
        // 顶层画布，用于承载全屏遮幕
        private Canvas canvas;

        // 黑色全屏图片，实现遮挡效果
        private Image black;

        // 透明度控制组件，用于淡入淡出
        private CanvasGroup cg;

        // 确保盖住一切
        private int sortingOrder = 9999;

        private Coroutine fadeCoroutine;

        private void OnEnable()
        {
            EventBus.Subscribe<ESceneFade>(OnSceneFade, false);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ESceneFade>(OnSceneFade);
        }

        private void BuildOverlay()
        {
            // 根节点
            var root = new GameObject("ScreenFader");
            root.transform.SetParent(transform);
            canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();

            // CanvasGroup 控制透明度
            cg = root.AddComponent<CanvasGroup>();
            cg.alpha = 0f;

            // 全屏黑图
            var imgGo = new GameObject("Black");
            imgGo.transform.SetParent(root.transform, false);
            black = imgGo.AddComponent<Image>();
            black.color = Color.black;
            black.raycastTarget = false;

            var rt = imgGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private void OnSceneFade(ESceneFade e)
        {
            StartCoroutine(Co_FadeAdditiveDisable(e));
        }

        private IEnumerator Co_FadeAdditiveDisable(ESceneFade e)
        {
            // 确保一次性构建
            if (canvas == null || black == null || cg == null)
            {
                BuildOverlay();
            }
            // 渐入进黑屏
            yield return FadeRoutine(cg.alpha, 1f, e.FadeOutDuration);

            if (!string.IsNullOrEmpty(e.ToScene))
            {
                SceneManager.LoadScene(e.ToScene, LoadSceneMode.Single);
                // 单场景加载是同步的；为了稳妥可让一帧再开始淡入
                yield return null;
            }
            // 淡入
            yield return FadeRoutine(1f, 0f, e.FadeInDuration);
        }

        // 淡出到黑：alpha 从 0（全透明）到 1（全黑），用时 duration
        public Coroutine FadeOut(float duration = 0.5f) => StartFade(0f, 1f, duration);

        // 淡入到亮：alpha 从 1（全黑）到 0（全透明），用时 duration
        public Coroutine FadeIn(float duration = 0.5f) => StartFade(1f, 0f, duration);

        private Coroutine StartFade(float from, float to, float duration)
        {
            // 确保一次性构建
            if (canvas == null || black == null || cg == null)
            {
                BuildOverlay();
            }
            // 若已有淡变在跑，先停止防止叠加
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeRoutine(from, to, duration));
            return fadeCoroutine;
        }

        // 实际执行淡变的协程
        private IEnumerator FadeRoutine(float from, float to, float duration)
        {
            float t = 0f;
            cg.alpha = from;
            // 淡变期屏蔽输入
            black.raycastTarget = true;

            while (t < duration)
            {
                // 不受 timeScale 影响
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }

            cg.alpha = to; // 强制收敛
            black.raycastTarget = (to > 0f);
        }
    }
}
