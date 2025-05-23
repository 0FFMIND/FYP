using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputSystem;
using AudioSystem;
using UnityEngine.UI;

namespace MVC
{
    [System.Serializable]
    public struct LineMapping
    {
        [Tooltip("当 index 等于此值时，切换到对应的 sprite")]
        public int lineIndex;
        [Tooltip("切换时使用的 Sprite")]
        public Sprite sprite;
    }
    public class DialogueCtl : MonoBehaviour
    {

        [Header("翻页箭头位移")]
        public Transform arrow;
        [SerializeField] private float arrowOffset;      // 首次定位的像素偏移
        [SerializeField] private int downFrames;      // 向下移动时等待帧数
        [SerializeField] private float downDistance;   // 向下移动的世界/本地单位
        [SerializeField] private int upFrames;      // 向上移动时等待帧数
        // 对话
        public DialogueView view;
        [SerializeField]
        private string dialogueTxt;
        [SerializeField]
        private LineMapping[] mappings;
        [SerializeField]
        private float typeSpeed;
        //
        private Sprite currentSprite;
        private int index;
        private DialogueModel model;
        private Coroutine typingCoroutine;
        private Coroutine arrowBounceCoroutine;

        public void StartDialogue()
        {
            // 清空现在有的内容
            arrow.gameObject.SetActive(false);
            view.Render(null, null);
            // 读取text"1-Scene-1.txt"
            model = new DialogueModel(dialogueTxt);
            // 刷新index
            index = 0;
            // 注册事件
            InputManager.Instance.OnAction += OnInputAction;
            AudioManager.Instance.PlayBGM("1-bgm");
            // 自动播放第一句话
            NextLine();
        }
        private void OnInputAction(InputAction action)
        {
            if(action == InputAction.DialogueClick)
            {
                if (typingCoroutine != null)
                {
                    // 先暂停
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                    if(index <= model.Lines.Length)
                    {
                        view.Render(currentSprite, model.Lines[index - 1]);
                        // 开启小箭头
                        PositionArrowUnderText();
                    }
                    else
                    {
                        // 结束的时候清空
                        view.Render(null, null);
                        // 关掉小箭头
                        arrow.gameObject.SetActive(false);
                    }
                }
                else
                {
                    NextLine();
                }
            }
        }
        private void NextLine()
        {
            // 如果读完，则清空
            if(index == model.Lines.Length)
            {
                view.Render(null, null);
                InputManager.Instance.OnAction -= OnInputAction;
                return;
            }
            foreach(var map in mappings)
            {
                if(index == map.lineIndex)
                {
                    currentSprite = map.sprite;
                    break;
                }
            }
            string text = model.Lines[index];
            // 打字
            typingCoroutine = StartCoroutine(TypeLines(text));
            // 移动到下一个line
            index++;
        }

        private IEnumerator TypeLines(string fullText)
        {
            // 关闭显示
            arrow.gameObject.SetActive(false);
            view.Render(currentSprite, "");
            for(int i = 0; i < fullText.Length; i++)
            {
                view.Render(currentSprite, fullText.Substring(0, i + 1));
                if(i < fullText.Length - 3)
                    AudioManager.Instance.PlaySFX("typing");
                yield return new WaitForSeconds(typeSpeed);
            }
            // 重新显示
            PositionArrowUnderText();
            typingCoroutine = null;

        }
        private void PositionArrowUnderText()
        {
            view.tmp.ForceMeshUpdate();
            Bounds b = view.tmp.textBounds;
            Vector3 localBotCenter = new Vector3(b.center.x, b.min.y, 0);
            Vector3 worldBotCenter = view.tmp.transform.TransformPoint(localBotCenter);
            Vector3 downOffset = Vector3.down * arrowOffset;
            arrow.position = new Vector3(arrow.position.x, worldBotCenter.y + downOffset.y, arrow.position.z);
            // 显示，并向下偏移
            arrow.gameObject.SetActive(true);
            // 启动抖动
            if (arrowBounceCoroutine != null) StopCoroutine(arrowBounceCoroutine);
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
                    float t = i / (float)downFrames;  // 从 0 到 1
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

        // 取消订阅
        private void OnDestroy()
        {
            if (arrowBounceCoroutine != null)
                StopCoroutine(arrowBounceCoroutine);
            InputManager.Instance.OnAction -= OnInputAction;
        }
    }
}

