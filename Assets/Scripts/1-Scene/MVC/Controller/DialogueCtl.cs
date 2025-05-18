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
        [Tooltip("�� index ���ڴ�ֵʱ���л�����Ӧ�� sprite")]
        public int lineIndex;
        [Tooltip("�л�ʱʹ�õ� Sprite")]
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
            // ��������е�����
            view.Render(null, null);
            // ��ȡtext"1-Scene-1.txt"
            model = new DialogueModel(dialogueTxt);
            // ˢ��index
            index = 0;
            // ע���¼�
            InputManager.Instance.OnAction += OnInputAction;
            AudioManager.Instance.PlayBGM("1-bgm");
            // �Զ����ŵ�һ�仰
            NextLine();
        }
        private void OnInputAction(InputAction action)
        {
            if(action == InputAction.DialogueClick)
            {
                if (typingCoroutine != null)
                {
                    // ����ͣ
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                    if(index <= model.Lines.Length)
                    {
                        view.Render(currentSprite, model.Lines[index - 1]);
                    }
                    else
                    {
                        // ������ʱ�����
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
            // ������꣬�����
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
            // ����
            typingCoroutine = StartCoroutine(TypeLines(text));
            // �ƶ�����һ��line
            index++;
        }

        private IEnumerator TypeLines(string fullText)
        {
            view.Render(currentSprite, "");
            // ����
            for(int i = 0; i < fullText.Length; i++)
            {
                view.Render(currentSprite, fullText.Substring(0, i + 1));
                if(i < fullText.Length - 3)
                    AudioManager.Instance.PlaySFX("typing");
                yield return new WaitForSeconds(typeSpeed);
            }
            typingCoroutine = null;

        }

        // ȡ������
        private void OnDestroy()
        {
            InputManager.Instance.OnAction -= OnInputAction;
        }
    }
}

