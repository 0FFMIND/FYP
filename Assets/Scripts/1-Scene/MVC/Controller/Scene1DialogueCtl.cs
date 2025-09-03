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
        shake,
        playHeartBeat,
        playHint,
        playBGM,
        arrowRed,
        stopBGM,
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

    public class Scene1DialogueCtl : MonoBehaviour
    {
        [Header("翻页箭头位移")]
        [SerializeField]
        private Transform arrow;

        [SerializeField]
        private float arrowOffset; // 首次定位的像素偏移

        [SerializeField]
        private int downFrames; // 向下移动时等待帧数

        [SerializeField]
        private float downDistance; // 向下移动的世界/本地单位

        [SerializeField]
        private int upFrames; // 向上移动时等待帧数

        // 对话
        [Header("ScriptableObject 对话资源")]
        // 1-Scene-1.txt
        [SerializeField]
        private string prologueTxt;

        [SerializeField]
        private DialogueView bgView;

        [SerializeField]
        private DialogueView dialogueView;

        [SerializeField]
        private LineMapping[] mappings;

        [SerializeField]
        private float typeSpeed;

        [SerializeField]
        private CameraShake cameraShake;

        //
        private DialogueModel dialogueModel;
        private Sprite currentSprite;
        private int index;
        private Coroutine typingCoroutine;
        private Coroutine arrowBounceCoroutine;

        public void HideDialogue()
        {
            // 隐藏内容
            arrow.gameObject.SetActive(false);
            RenderViews(null, null);
        }

        public void RenderViews(Sprite sprite, string text)
        {
            bgView.Render(sprite, null);
            dialogueView.Render(null, text);
        }

        public void StartDialogue()
        {
            // bgm
            AudioManager.Instance.PlayBGM("1-bgm", 0f);
            // 载入对话
            dialogueModel = new DialogueModel(prologueTxt);
            // 刷新index
            index = 0;
            // 注册事件
            EventBus.Subscribe<EInputPressed, InputAction>(InputAction.DialogueClick, OnDialogueClick);
            NextLine();
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
                    RenderViews(currentSprite, dialogueModel.Lines[index - 1]);
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

        private void NextLine()
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.white;
            // 如果读完
            if (index == dialogueModel.Lines.Length)
            {
                // 进入1-Scene-Main
                SceneMgr.Instance.LoadScenesAdditive("1-Scene-Main");

                SceneMgr.Instance.DisableScene("1-Scene-UI");
            }
            // 不然按钮点击会误认为nextline
            if (dialogueModel == null || index >= dialogueModel.Lines.Length)
            {
                return;
            }
            foreach (var map in mappings)
            {
                if (index == map.lineIndex)
                {
                    currentSprite = map.sprite;
                    foreach (Eact eact in map.eacts)
                    {
                        if (eact != Eact.none)
                        {
                            if (eact == Eact.shake)
                            {
                                // 调用camera shake并播放音效
                                AudioManager.Instance.PlaySFX("shocked");
                                cameraShake.Shake();
                            }
                            if (eact == Eact.playBGM)
                            {
                                AudioManager.Instance.PlayBGM("1-bgm");
                            }
                            if (eact == Eact.arrowRed)
                            {
                                arrow.GetComponent<SpriteRenderer>().color = Color.red;
                            }
                            if (eact == Eact.playHeartBeat)
                            {
                                AudioManager.Instance.PlaySFX("heartbeat");
                            }
                            if (eact == Eact.stopBGM)
                            {
                                AudioManager.Instance.StopBGM();
                            }
                        }
                    }

                    break;
                }
            }
            string text = dialogueModel.Lines[index];
            // 打字
            typingCoroutine = StartCoroutine(TypeLines(text));
            // 移动到下一个line
            index++;
        }

        private IEnumerator TypeLines(string fullText)
        {
            arrow.gameObject.SetActive(false);
            RenderViews(currentSprite, "");

            string displayed = "";
            var openTags = new Stack<string>();
            bool inHint = false; // 标记当前是否在 hint 区域

            for (int i = 0; i < fullText.Length; i++)
            {
                if (fullText[i] == '<')
                {
                    int close = fullText.IndexOf('>', i);
                    if (close != -1)
                    {
                        string rawTag = fullText.Substring(i, close - i + 1);
                        bool isCloseTag = rawTag.StartsWith("</");
                        string inner = rawTag.Trim('<', '/', '>');
                        string tagName = inner.Split(new[] { ' ', '=' }, 2)[0];

                        if (!isCloseTag && tagName == "hint")
                        {
                            // 进入 hint 区域
                            inHint = true;
                            AudioManager.Instance.PlaySFX("shocked");
                            displayed += "<color=#FF0000>";
                            openTags.Push("color");
                        }
                        else if (isCloseTag && tagName == "hint")
                        {
                            // 离开 hint 区域
                            inHint = false;
                            displayed += "</color>";
                            if (openTags.Count > 0)
                                openTags.Pop();
                        }
                        else
                        {
                            // 普通标签处理
                            displayed += rawTag;
                            if (isCloseTag)
                            {
                                if (openTags.Count > 0)
                                    openTags.Pop();
                            }
                            else if (!rawTag.EndsWith("/>"))
                            {
                                int sep = rawTag.IndexOfAny(new[] { ' ', '=', '>' }, 1);
                                string name = rawTag.Substring(
                                    1,
                                    (sep > 0 ? sep : rawTag.Length - 2) - 1
                                );
                                openTags.Push(name);
                            }
                        }

                        i = close;
                    }
                    else
                    {
                        displayed += fullText[i];
                    }

                    // 标签无需打字音效，立即渲染
                    RenderViews(currentSprite, displayed);
                    yield return null;
                }
                else
                {
                    // 普通文字
                    displayed += fullText[i];
                    AudioManager.Instance.PlaySFX("typing");

                    // 拼接闭合标签保证 TMP 正确渲染
                    string temp = displayed;
                    foreach (var name in openTags)
                        temp += $"</{name}>";

                    RenderViews(currentSprite, temp);

                    // 不同文本用不同速度
                    float wait = inHint ? typeSpeed * 3f : typeSpeed;
                    if (LocalizationMgr.Instance.CurrentLanguage == "en")
                    {
                        // 英文要快一点
                        wait = wait / 2;
                    }
                    yield return new WaitForSeconds(wait);
                }
            }

            // 完成后显示箭头
            PositionArrowUnderText();
            typingCoroutine = null;
        }

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
                StopCoroutine(arrowBounceCoroutine);
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

        private void OnDisable()
        {
            EventBus.Unsubscribe<EInputPressed, InputAction>(InputAction.DialogueClick, OnDialogueClick);
        }

        // 取消订阅
        private void OnDestroy()
        {
            if (arrowBounceCoroutine != null)
                StopCoroutine(arrowBounceCoroutine);
        }
    }
}
