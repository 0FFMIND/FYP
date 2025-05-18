using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVC.View;
using MVC.Model;
using InputSystem;
using AudioSystem;

namespace MVC.Controller
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
        [SerializeField]
        private DialogueView view;
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
        private void Start()
        {
            // 清空现在有的内容
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
                    }
                    else
                    {
                        // 结束的时候清空
                        view.Render(null, null);
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
            view.Render(currentSprite, "");
            // 声音
            for(int i = 0; i < fullText.Length; i++)
            {
                view.Render(currentSprite, fullText.Substring(0, i + 1));
                if(i < fullText.Length - 3)
                    AudioManager.Instance.PlaySFX("typing");
                yield return new WaitForSeconds(typeSpeed);
            }
            typingCoroutine = null;

        }

        // 取消订阅
        private void OnDestroy()
        {
            InputManager.Instance.OnAction -= OnInputAction;
        }
    }
}

