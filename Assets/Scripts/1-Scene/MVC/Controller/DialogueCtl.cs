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
        [Tooltip("�� index ���ڴ�ֵʱ���л�����Ӧ�� sprite")]
        public int lineIndex;
        [Tooltip("�л�ʱʹ�õ� Sprite")]
        public Sprite sprite;
    }
    public class DialogueCtl : MonoBehaviour
    {

        [Header("��ҳ��ͷλ��")]
        public Transform arrow;
        [SerializeField] private float arrowOffset;      // �״ζ�λ������ƫ��
        [SerializeField] private int downFrames;      // �����ƶ�ʱ�ȴ�֡��
        [SerializeField] private float downDistance;   // �����ƶ�������/���ص�λ
        [SerializeField] private int upFrames;      // �����ƶ�ʱ�ȴ�֡��
        // �Ի�
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
            // ��������е�����
            arrow.gameObject.SetActive(false);
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
                        // ����С��ͷ
                        PositionArrowUnderText();
                    }
                    else
                    {
                        // ������ʱ�����
                        view.Render(null, null);
                        // �ص�С��ͷ
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
            // �ر���ʾ
            arrow.gameObject.SetActive(false);
            view.Render(currentSprite, "");
            for(int i = 0; i < fullText.Length; i++)
            {
                view.Render(currentSprite, fullText.Substring(0, i + 1));
                if(i < fullText.Length - 3)
                    AudioManager.Instance.PlaySFX("typing");
                yield return new WaitForSeconds(typeSpeed);
            }
            // ������ʾ
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
            // ��ʾ��������ƫ��
            arrow.gameObject.SetActive(true);
            // ��������
            if (arrowBounceCoroutine != null) StopCoroutine(arrowBounceCoroutine);
            arrowBounceCoroutine = StartCoroutine(ArrowBounce());
        }
        private IEnumerator ArrowBounce()
        {
            // ��¼ԭʼλ��
            Vector3 original = arrow.position;
            Vector3 target = original + Vector3.down * downDistance;
            while (true)
            {
                // ƽ������
                for (int i = 0; i <= downFrames; i++)
                {
                    float t = i / (float)downFrames;  // �� 0 �� 1
                    arrow.position = Vector3.Lerp(original, target, t);
                    yield return null;
                }
                // ƽ������
                for (int i = 0; i <= upFrames; i++)
                {
                    float t = i / (float)upFrames;
                    arrow.position = Vector3.Lerp(target, original, t);
                    yield return null;
                }
            }
        }

        // ȡ������
        private void OnDestroy()
        {
            if (arrowBounceCoroutine != null)
                StopCoroutine(arrowBounceCoroutine);
            InputManager.Instance.OnAction -= OnInputAction;
        }
    }
}

