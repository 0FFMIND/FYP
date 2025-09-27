using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace MVC
{
    public enum Eact
    {
        none,
        playBGM,
        arrowRed,
    }

    [System.Serializable]
    public struct LineMapping
    {
        [Tooltip("当 index 等于此值时，切换到对应的 sprite")]
        public int lineIndex;

        [Tooltip("切换时使用的 Sprite")]
        public Sprite sprite;

        [Tooltip("触发行为")]
        public Eact[] eacts;
    }

    public abstract class DialogCtlBase : MonoBehaviour
    {
        [Header("Dialogue 面板入场")]
        [SerializeField]
        protected float panelEnterDuration; // 入场时长(秒)

        [SerializeField]
        protected float panelEnterOffsetY;

        [SerializeField]
        protected float panelEnterOffsetX;

        [SerializeField]
        protected AnimationCurve panelEnterCurve = null; // 可选缓动

        [Header("翻页箭头位移")]
        [SerializeField]
        protected float arrowOffset; // 首次定位的像素偏移

        [SerializeField]
        protected int downFrames; // 向下移动时等待帧数

        [SerializeField]
        protected float downDistance; // 向下移动的世界/本地单位

        [SerializeField]
        protected int upFrames; // 向上移动时等待帧数

        [Header("ScriptableObject 对话资源")]
        [SerializeField]
        protected string modelText;

        [SerializeField]
        protected LineMapping[] mappings;

        [Header("视图引用")]
        [SerializeField]
        protected DialogueView bgView;

        [SerializeField]
        protected DialogueView dialogueView;

        protected Transform arrow;
        protected int index;
        protected DialogueModel dialogueModel;
        protected Sprite currentSprite;
        protected Coroutine typingCoroutine;

        private Image panelImage; // 指向 dialogueView 下的 Panel 的 Image
        private float typeSpeed;
        private Coroutine arrowBounceCoroutine;
        private float _alphaTarget;

        public virtual void StartDialogue()
        {
            // 创建down arrow
            CreateArrow();
            // 载入对话
            dialogueModel = new DialogueModel(modelText);
            // 刷新index
            index = 0;
            // 注册事件
            EventBus.Subscribe<EInputPressed, InputAction>(
                InputAction.DialogueClick,
                OnDialogueClick
            );
            NextLine();
        }

        protected IEnumerator SlideInDialogueView()
        {
            panelImage = dialogueView.GetComponentInChildren<Image>(true);
            var c = panelImage.color;
            _alphaTarget = c.a;
            c.a = 0f;
            panelImage.color = c;
            if (!dialogueView)
            {
                yield break;
            }
            var go = dialogueView.gameObject;
            // 入场前先确保面板可见，箭头隐藏
            if (!go.activeSelf)
            {
                go.SetActive(true);
            }
            if (arrow)
            {
                arrow.gameObject.SetActive(false);
            }

            float dur = Mathf.Max(0.0001f, panelEnterDuration);
            float t = 0f;
            // 使用 RectTransform 优先（UI），否则走 Transform.localPosition（世界/局部物体）
            var rt = go.transform as RectTransform;
            if (panelEnterCurve == null)
                panelEnterCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 默认S型缓入缓出

            if (rt != null)
            {
                Vector2 target = rt.anchoredPosition;
                Vector2 start = target + new Vector2(panelEnterOffsetX, -panelEnterOffsetY);
                rt.anchoredPosition = start;
                while (t < dur)
                {
                    t += Time.unscaledDeltaTime;
                    float u = panelEnterCurve.Evaluate(Mathf.Clamp01(t / dur)); // 由曲线决定进度
                    rt.anchoredPosition = Vector2.LerpUnclamped(start, target, u); // 允许曲线>1产生回弹
                    c = panelImage.color;
                    c.a = Mathf.LerpUnclamped(0f, _alphaTarget, u);
                    panelImage.color = c;
                    yield return null;
                }
                rt.anchoredPosition = target;
            }
            else
            {
                Vector3 target = go.transform.localPosition;
                Vector3 start = target + new Vector3(panelEnterOffsetX, -panelEnterOffsetY, 0f);
                go.transform.localPosition = start;
                while (t < dur)
                {
                    t += Time.unscaledDeltaTime;
                    float u = panelEnterCurve.Evaluate(Mathf.Clamp01(t / dur));
                    go.transform.localPosition = Vector3.LerpUnclamped(start, target, u);
                    yield return null;
                }
                go.transform.localPosition = target;
            }
        }

        private void CreateArrow()
        {
            if (arrow != null)
            {
                return;
            }
            var prefab = Resources.Load<GameObject>("Prefabs/1-Scene/DownRow");
            if (!prefab)
            {
                Debug.LogError($"[DialogCtlBase] Resources.Load 失败");
                return;
            }

            // 实例化并挂到dialog上
            var go = Instantiate(prefab, transform);

            // 记录 Transform
            arrow = go.transform;
            arrow.gameObject.SetActive(false);
        }

        protected virtual void OnDialogueFinished() { }

        // 基类提供统一渲染方法，子类可直接调用
        protected void RenderViews(Sprite sprite, string text)
        {
            if (bgView)
            {
                bgView.Render(sprite, null);
            }
            if (dialogueView)
            {
                if (text == null)
                {
                    dialogueView.gameObject.SetActive(false);
                }
                else
                {
                    if (!dialogueView.gameObject.activeSelf)
                    {
                        dialogueView.gameObject.SetActive(true);
                    }
                    dialogueView.Render(null, text);
                }
            }
        }

        private void RevealAllNow()
        {
            var tmp = dialogueView.tmp;
            if (!tmp)
                return;

            // 直接拉满可见字符，避免依赖 textInfo 的计数时机
            tmp.maxVisibleCharacters = int.MaxValue;

            // 可选：如果你需要用到 characterCount，再强制刷新一次
            tmp.ForceMeshUpdate();

            PositionArrowUnderText();
        }

        protected virtual IEnumerator TypeLines(string fullRaw)
        {
            // 一次性设置完整文本，然后用 maxVisibleCharacters 揭示
            RenderViews(currentSprite, fullRaw);
            var tmp = dialogueView.tmp;
            tmp.ForceMeshUpdate();
            tmp.maxVisibleCharacters = 0;

            int total = tmp.textInfo.characterCount;
            int cnt = 0;

            for (int vis = 1; vis <= total; vis++)
            {
                tmp.maxVisibleCharacters = vis;

                // 英文两字符一个音效；中文每个字符一个音效
                bool isEn = SettingsMgr.Instance.GetLanguage() == LanguageCode.en;
                if (isEn)
                {
                    cnt++;
                    if (cnt >= 2)
                    {
                        cnt = 0;
                        AudioManager.Instance.PlaySFX("typing");
                    }
                }
                else
                {
                    AudioManager.Instance.PlaySFX("typing");
                }
                float wait = typeSpeed;
                if (isEn)
                    wait *= 0.5f;
                yield return new WaitForSeconds(wait);
            }

            // 完成后显示箭头
            PositionArrowUnderText();
            typingCoroutine = null;
        }

        private void OnDialogueClick()
        {
            if (typingCoroutine != null)
            {
                // 先暂停
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                if (index <= dialogueModel.Lines.Length)
                {
                    RevealAllNow();
                    // 开启小箭头
                    PositionArrowUnderText();
                }
                else
                {
                    // 结束的时候清空
                    RenderViews(null, null);
                    // 关掉小箭头
                    arrow.gameObject.SetActive(false);
                    arrow.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            else
            {
                NextLine();
            }
        }

        // 由子类实现：推进到下一行
        protected abstract void NextLine();

        private void PositionArrowUnderText()
        {
            dialogueView.tmp.ForceMeshUpdate();
            Bounds b = dialogueView.tmp.textBounds;
            Vector3 localBotCenter = new Vector3(b.center.x, b.min.y, 0);
            Vector3 worldBotCenter = dialogueView.tmp.transform.TransformPoint(localBotCenter);
            Vector3 downOffset = Vector3.down * arrowOffset;
            arrow.position = new Vector3(
                arrow.position.x,
                worldBotCenter.y + downOffset.y,
                arrow.position.z
            );
            // 显示，并向下偏移
            arrow.gameObject.SetActive(true);
            // 启动抖动
            if (arrowBounceCoroutine != null)
            {
                StopCoroutine(arrowBounceCoroutine);
            }
            arrowBounceCoroutine = StartCoroutine(ArrowBounce());
        }

        private IEnumerator ArrowBounce()
        {
            // 记录原始位置
            Vector3 original = arrow.position;
            Vector3 target = original + Vector3.down * downDistance;
            while (true)
            {
                // 平滑下移
                for (int i = 0; i <= downFrames; i++)
                {
                    float t = i / (float)downFrames; // 从 0 到 1
                    arrow.position = Vector3.Lerp(original, target, t);
                    yield return null;
                }
                // 平滑上移
                for (int i = 0; i <= upFrames; i++)
                {
                    float t = i / (float)upFrames;
                    arrow.position = Vector3.Lerp(target, original, t);
                    yield return null;
                }
            }
        }

        protected virtual void OnEnable()
        {
            EventBus.Subscribe<ESettingsChanged>(OnSettingsChanged);
        }

        private void OnSettingsChanged(ESettingsChanged e)
        {
            typeSpeed = e.Settings.typeSpeed;
        }

        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<ESettingsChanged>(OnSettingsChanged);
            EventBus.Unsubscribe<EInputPressed, InputAction>(
                InputAction.DialogueClick,
                OnDialogueClick
            );
        }
    }
}
