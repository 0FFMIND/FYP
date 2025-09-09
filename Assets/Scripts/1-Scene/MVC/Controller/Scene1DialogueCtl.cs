using System.Collections;
using Manager;
using UnityEngine;
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

        private void RevealAllNow()
        {
            var tmp = dialogueView.tmp;
            if (!tmp) return;

            // 直接拉满可见字符，避免依赖 textInfo 的计数时机
            tmp.maxVisibleCharacters = int.MaxValue;

            // 可选：如果你需要用到 characterCount，再强制刷新一次
            tmp.ForceMeshUpdate();

            PositionArrowUnderText();
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

        private void NextLine()
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.white;
            // 如果读完
            if (index == dialogueModel.Lines.Length)
            {
                // 进入1-Scene-Main
                SceneMgr.Instance.LoadScenesAdditive("1-Scene-Main");

                SceneMgr.Instance.DisableScene("1-Scene-UI");
                // 可以暂停
                PauseMgr.Instance.SetPauseEnabled(true);

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

        private IEnumerator TypeLines(string fullRaw)
        {
            arrow.gameObject.SetActive(false);

            // 一次性设置完整文本，然后用 maxVisibleCharacters 揭示
            RenderViews(currentSprite, fullRaw);
            var tmp = dialogueView.tmp;
            tmp.ForceMeshUpdate();                 // 让 TMP 生成 textInfo
            tmp.maxVisibleCharacters = 0;          // 从 0 开始揭示

            int total = tmp.textInfo.characterCount; // 不包含富文本标签字符
            int cnt = 0;                             // 控制英文每两个字符播一次音效

            for (int vis = 1; vis <= total; vis++)
            {
                tmp.maxVisibleCharacters = vis;

                // 英文两字符一个音效；中文每个字符一个音效
                bool isEn = SettingsMgr.Instance.GetLanguage() == LanguageCode.en;
                if (isEn) { cnt++; if (cnt >= 2) { cnt = 0; AudioManager.Instance.PlaySFX("typing"); } }
                else { AudioManager.Instance.PlaySFX("typing"); }

                float wait = typeSpeed;
                if (isEn) wait *= 0.5f;            // 英文更快
                yield return new WaitForSeconds(wait);
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
