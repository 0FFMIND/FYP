using System.Collections;
using UnityEngine;
using Utils; // 仅用于 EPauseChanged（可去掉）

namespace MVC
{
    public enum EmoteType
    {
        Thinking,
        Eyes,
        Checked,
    }

    /// <summary>
    /// 头顶表情/思考气泡（序列帧动画）
    /// 使用：把本组件挂到角色上，在 Inspector 里把两组帧拖进去；
    /// 代码里调用 Play(EmoteType.Thinking) 或 Play(EmoteType.Eyes) 即可。
    /// </summary>
    public class PlayerEmoteCtl : MonoBehaviour
    {
        [Header("动画节奏")]
        [SerializeField]
        private float frameRate = 0.10f; // 每帧间隔

        [Header("序列帧")]
        [SerializeField]
        private Sprite[] thinkingSprites;

        [SerializeField]
        private Sprite[] eyesSprites;

        [SerializeField]
        private Sprite[] checkedSprites;

        [Header("入场 / 退场")]
        [SerializeField]
        private float slideInDuration = 0.18f; // 入场时长

        [SerializeField]
        private float slideOutDuration = 0.15f; // 退场时长

        [SerializeField]
        private Vector2 slideInOffset = new Vector2(0f, -0.25f); // 入场起始额外偏移（相对目标位置）

        [SerializeField]
        private Vector2 slideOutOffset = new Vector2(0f, -0.15f); // 退场结束额外偏移（相对目标位置）

        [SerializeField]
        private AnimationCurve slideInCurve = null; // 入场缓动曲线（为空则默认 EaseInOut）

        [SerializeField]
        private AnimationCurve slideOutCurve = null; // 退场缓动曲线（为空则默认 EaseInOut）

        [SerializeField]
        private float startAlpha = 0f; // 入场起始透明度

        [SerializeField]
        private float targetAlpha = 1f; // 入场目标透明度

        // 头顶气泡的渲染器
        [SerializeField]
        private SpriteRenderer bubbleSR;

        [SerializeField]
        private Vector2 localOffset = new Vector2(0f, 1.2f);

        private Sprite[] currentAnim;
        private int frameIndex;
        private float timer;
        private float lifeTimer;
        private float lifeLimit; // 持续时长
        private bool isPaused;
        private Coroutine slideCo; // 当前入场/退场协程
        private Vector3 baseLocalPos;
        private float rate = 0.10f; // 每帧间隔

        private void Start()
        {
            bubbleSR.enabled = false; // 初始隐藏
            // 目标锚点 = 父坐标系下的本地偏移（Z 保持当前）
            baseLocalPos = new Vector3(
                localOffset.x,
                localOffset.y,
                bubbleSR.transform.localPosition.z
            );
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EPauseChanged>(OnPauseChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EPauseChanged>(OnPauseChanged);
        }

        private void OnPauseChanged(EPauseChanged e) => isPaused = e.IsPaused;

        private void LateUpdate()
        {
            // 暂停时不更新任何计时
            if (isPaused)
            {
                return;
            }
            if (currentAnim == null || currentAnim.Length == 0)
            {
                return;
            }
            // 播放帧
            timer += Time.deltaTime;
            if (timer >= rate)
            {
                timer = 0f;
                frameIndex = (frameIndex + 1) % currentAnim.Length;
                bubbleSR.sprite = currentAnim[frameIndex];
            }
            // 若设置了 lifeLimit（>0），则推进寿命计时器
            if (lifeLimit > 0f)
            {
                lifeTimer += Time.deltaTime;
                if (lifeTimer >= lifeLimit)
                {
                    Stop();
                }
            }
        }

        // 播放指定类型的表情
        public void Play(EmoteType type, float duration = -1f, bool skipIn = false)
        {
            if (type == EmoteType.Eyes)
            {
                rate = frameRate * 1.5f;
            }
            else
            {
                rate = frameRate;
            }
            currentAnim = GetAnim(type);

            frameIndex = 0;
            timer = 0f;
            lifeLimit = duration;
            lifeTimer = 0f;
            bubbleSR.enabled = true;
            bubbleSR.sprite = currentAnim[0];

            // 触发入场（如已在入/退场，先打断）
            SlideIn(skipIn);
        }

        // 停止并隐藏
        public void Stop()
        {
            SlideOut();
        }

        public void SlideIn(bool skip = false)
        {
            // 若有正在运行的入/退场协程，先停止
            if (slideCo != null)
                StopCoroutine(slideCo);

            if (skip)
            {
                // 直接设置到目标状态（位置+透明度），不跑协程
                Color c = bubbleSR.color;
                bubbleSR.transform.localPosition = baseLocalPos;
                c.a = targetAlpha;
                bubbleSR.color = c;
                bubbleSR.enabled = true;
                slideCo = null;
            }
            else
            {
                slideCo = StartCoroutine(SlideInRoutine());
            }
        }

        public void SlideOut()
        {
            // 若有正在运行的入/退场协程，先停止
            if (slideCo != null)
                StopCoroutine(slideCo);

            // 开启新的退场协程
            slideCo = StartCoroutine(SlideOutRoutine());
        }

        private IEnumerator SlideInRoutine()
        {
            Vector3 targetPos = baseLocalPos;

            // 入场起点 = 目标位置 + 额外偏移（注意 y 方向）
            Vector3 startPos = targetPos + new Vector3(slideInOffset.x, slideInOffset.y, 0f);

            // 读取并暂存初始颜色
            Color c = bubbleSR.color;

            // 设置初始状态：定位到起点、渲染开启、初始透明度
            bubbleSR.transform.localPosition = startPos;
            bubbleSR.enabled = true;
            c.a = startAlpha;
            bubbleSR.color = c;

            // 计时
            float dur = Mathf.Max(0.0001f, slideInDuration);
            float t = 0f;

            // 动画主循环
            while (t < dur)
            {
                // 暂停时冻结计时
                if (!isPaused)
                    t += Time.unscaledDeltaTime;

                // 归一化进度 [0,1]
                float u = Mathf.Clamp01(t / dur);

                // 根据缓动曲线取样
                float k = slideInCurve.Evaluate(u);

                // 位置插值（允许曲线 >1 带回弹）
                bubbleSR.transform.localPosition = Vector3.LerpUnclamped(startPos, targetPos, k);

                // 透明度插值
                c.a = Mathf.LerpUnclamped(startAlpha, targetAlpha, k);
                bubbleSR.color = c;

                // 等下一帧
                yield return null;
            }

            // 收尾：对齐最终值
            bubbleSR.transform.localPosition = targetPos;
            c.a = targetAlpha;
            bubbleSR.color = c;

            // 协程结束，清空引用
            slideCo = null;
        }

        // 退场协程：从目标位置移动到（目标位置 + slideOutOffset），并从当前 alpha 淡到 0
        private IEnumerator SlideOutRoutine()
        {
            // 目标与起点
            Vector3 startLocal = bubbleSR.transform.localPosition; // 当前本地位置
            Vector3 targetLocal =
                baseLocalPos + new Vector3(slideOutOffset.x, slideOutOffset.y, 0f); // 退场终点

            // 颜色读写
            Color c = bubbleSR.color;
            float alphaStart = c.a; // 记录当前透明度作为退场起点
            float alphaEnd = 0f; // 退场后透明

            // 计时
            float dur = Mathf.Max(0.0001f, slideOutDuration);
            float t = 0f;

            // 动画主循环
            while (t < dur)
            {
                // 暂停时冻结计时
                if (!isPaused)
                    t += Time.unscaledDeltaTime;

                // 归一化进度 [0,1]
                float u = Mathf.Clamp01(t / dur);

                // 曲线采样
                float k = slideOutCurve.Evaluate(u);

                // 位置插值
                bubbleSR.transform.localPosition = Vector3.LerpUnclamped(
                    startLocal,
                    targetLocal,
                    k
                );

                // 透明度插值
                c.a = Mathf.LerpUnclamped(alphaStart, alphaEnd, k);
                bubbleSR.color = c;

                // 等下一帧
                yield return null;
            }

            // 收尾：关闭渲染器、重置到目标本地位置
            bubbleSR.transform.localPosition = targetLocal;
            c.a = 0f;
            bubbleSR.color = c;
            bubbleSR.enabled = false;
            currentAnim = null;

            // 协程结束，清空引用
            slideCo = null;
        }

        // 根据类型返回对应的序列帧数组
        private Sprite[] GetAnim(EmoteType t)
        {
            switch (t)
            {
                case EmoteType.Thinking:
                    return thinkingSprites;
                case EmoteType.Eyes:
                    return eyesSprites;
                case EmoteType.Checked:
                    return checkedSprites;
                default:
                    return null;
            }
        }
    }
}
