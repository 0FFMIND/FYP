using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public class EnterAnim : MonoBehaviour
    {
        [System.Serializable]
        public class TweenProfile
        {
            [Min(0.0001f)]
            public float duration = 0.25f;
            public float offsetX = 0f;
            public float offsetY = 0f;
            public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public enum Mode
        {
            CodeTween,
            AnimatorState,
        }

        [Header("Mode")]
        [SerializeField]
        private Mode mode = Mode.CodeTween;

        [Header("Code Tween")]
        [SerializeField]
        private TweenProfile dialogProfile;

        [SerializeField]
        private TweenProfile bgProfile;

        [Header("Animator State")]
        [SerializeField]
        private Animator animator;

        [SerializeField]
        public GameObject target;

        private int animLayer = 0;

        [SerializeField]
        private string enterState;

        [SerializeField]
        private string exitState;

        [SerializeField]
        private float enterWaitSeconds = 0f;

        [SerializeField]
        private float exitWaitSeconds = 0f;

        void Reset()
        {
            target = gameObject;
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (mode == Mode.AnimatorState)
            {
                target.gameObject.SetActive(false);
            }
        }

        public IEnumerator PlayEnterCode(DialogueView view, bool isBG)
        {
            if (mode != Mode.CodeTween)
            {
                // warning
            }
            var panelImage = view.GetComponentInChildren<Image>(true);
            var c = panelImage.color;
            var target = c.a;
            c.a = 0f;
            panelImage.color = c;
            if (!view)
            {
                yield break;
            }
            var go = view.gameObject;
            // 入场前先确保面板可见，箭头隐藏
            if (!go.activeSelf)
            {
                go.SetActive(true);
            }
            if (!isBG)
            {
                yield return SlideIn(
                    go,
                    panelImage,
                    target,
                    dialogProfile.duration,
                    dialogProfile.curve,
                    dialogProfile.offsetX,
                    dialogProfile.offsetY
                );
            }
            else
            {
                yield return SlideIn(
                    go,
                    panelImage,
                    target,
                    bgProfile.duration,
                    bgProfile.curve,
                    bgProfile.offsetX,
                    bgProfile.offsetY
                );
            }
        }

        public IEnumerator PlayEnterAnim(string state, float waitSeconds)
        {
            if (mode != Mode.AnimatorState)
            {
                // warning
            }
            yield return PlayAnimState(state, waitSeconds);
        }

        public IEnumerator PlayEnterAnim()
        {
            if (mode != Mode.AnimatorState)
            {
                // warning
            }
            yield return PlayAnimState(enterState, enterWaitSeconds);
        }

        public IEnumerator PlayExit()
        {
            if (mode != Mode.CodeTween)
            {
                // warning
            }
            yield return PlayAnimState(exitState, exitWaitSeconds);
            target.SetActive(false);
        }

        public IEnumerator PlayExitCode(DialogueView view, bool isBG)
        {
            if (mode != Mode.CodeTween)
                yield break;
            if (!view)
                yield break;

            var go = view.gameObject;
            var panelImage = view.GetComponentInChildren<Image>(true);
            var prof = isBG ? bgProfile : dialogProfile;
            var img = view.GetComponentInChildren<Image>(true);

            float keepAlpha = img ? img.color.a : 1f;

            // 从当前位置滑到“入场起点位置”
            yield return SlideOut(
                go,
                panelImage,
                keepAlpha,
                prof.duration,
                prof.curve,
                prof.offsetX,
                prof.offsetY
            );
        }

        private IEnumerator SlideOut(
            GameObject go,
            Image img,
            float alphaTarget,
            float duration,
            AnimationCurve curve,
            float offsetX,
            float offsetY
        )
        {
            float fadeOutAlpha = 0f;
            float dur = Mathf.Max(0.0001f, duration);
            float t = 0f;
            curve = AnimationCurve.EaseInOut(1, 1, 0, 0);

            var rt = go.transform as RectTransform;
            if (rt != null)
            {
                Vector2 start = rt.anchoredPosition; // 退场前的“基准位”
                Vector2 target = start + new Vector2(offsetX, -offsetY); // 退场终点（= 入场起点）

                float a0 = img ? img.color.a : 1f;

                while (t < dur)
                {
                    t += Time.unscaledDeltaTime;
                    float u = curve.Evaluate(Mathf.Clamp01(t / dur));
                    rt.anchoredPosition = Vector2.LerpUnclamped(start, target, u);

                    if (img)
                    {
                        var c = img.color;
                        c.a = Mathf.LerpUnclamped(a0, fadeOutAlpha, u);
                        img.color = c;
                    }
                    yield return null;
                }

                img.gameObject.SetActive(false);

                // 先抵达退场终点，再**复位回基准位**，防止下次进场累加偏移
                rt.anchoredPosition = target;
                if (img)
                {
                    var c = img.color;
                    c.a = alphaTarget;
                    img.color = c;
                }

                rt.anchoredPosition = start;
            }
            else
            {
                Vector3 start = go.transform.localPosition;
                Vector3 target = start + new Vector3(offsetX, -offsetY, 0f);

                while (t < dur)
                {
                    t += Time.unscaledDeltaTime;
                    float u = curve.Evaluate(Mathf.Clamp01(t / dur));
                    go.transform.localPosition = Vector3.LerpUnclamped(start, target, u);
                    yield return null;
                }

                // 同样复位
                go.transform.localPosition = target;
                go.transform.localPosition = start;
            }
        }

        private IEnumerator SlideIn(
            GameObject go,
            Image img,
            float alphaTarget,
            float duration,
            AnimationCurve curve,
            float offsetX,
            float offsetY
        )
        {
            float dur = Mathf.Max(0.0001f, duration);
            float t = 0f;
            // 使用 RectTransform 优先（UI），否则走 Transform.localPosition（世界/局部物体）
            var rt = go.transform as RectTransform;
            if (curve == null)
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            if (rt != null)
            {
                Vector2 target = rt.anchoredPosition;
                Vector2 start = target + new Vector2(offsetX, -offsetY);
                rt.anchoredPosition = start;
                while (t < dur)
                {
                    t += Time.unscaledDeltaTime;
                    float u = curve.Evaluate(Mathf.Clamp01(t / dur)); // 由曲线决定进度
                    rt.anchoredPosition = Vector2.LerpUnclamped(start, target, u); // 允许曲线>1产生回弹
                    var c = img.color;
                    c.a = Mathf.LerpUnclamped(0f, alphaTarget, u);
                    img.color = c;
                    yield return null;
                }
                rt.anchoredPosition = target;
            }
            else
            {
                Vector3 target = go.transform.localPosition;
                Vector3 start = target + new Vector3(offsetX, -offsetY, 0f);
                go.transform.localPosition = start;
                while (t < dur)
                {
                    t += Time.unscaledDeltaTime;
                    float u = curve.Evaluate(Mathf.Clamp01(t / dur));
                    go.transform.localPosition = Vector3.LerpUnclamped(start, target, u);
                    yield return null;
                }
                go.transform.localPosition = target;
            }
        }

        private IEnumerator PlayAnimState(string stateName, float waitSeconds)
        {
            target.SetActive(true);
            animator.gameObject.SetActive(true);
            animator.Play(stateName, animLayer, 0f);
            yield return new WaitForSeconds(waitSeconds);
        }
    }
}
