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
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 默认S型缓入缓出

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
