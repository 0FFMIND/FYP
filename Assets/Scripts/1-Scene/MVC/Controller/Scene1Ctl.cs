using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

namespace MVC
{
    public class Scene1Ctl : MonoBehaviour
    {
        [SerializeField]
        private Canvas choiceCanvas;
        [SerializeField]
        private DialogueView view;
        // 1-Scene-warning.txt
        [SerializeField]
        private string dialogueTxt;
        // �Ի���������warning���������
        [SerializeField]
        private DialogueCtl dialogueCtl;
        //
        private DialogueModel model;
        // ����canvasgroup����������͸����
        private CanvasGroup viewGroup;
        [SerializeField]
        private float fadeInDuration;
        [SerializeField]
        private float fadeOutDuration;
        [SerializeField, Tooltip("ͣ��ʱ�����룩")]
        private float holdDuration;
        private void Awake()
        {
            viewGroup = view.GetComponent<CanvasGroup>();
        }
        void Start()
        {
            choiceCanvas.gameObject.SetActive(false);
            // ��ȡtext"1-Scene-warning.txt"
            model = new DialogueModel(dialogueTxt);
            dialogueCtl.arrow.gameObject.SetActive(false);
            dialogueCtl.view.Render(null, null);
            view.Render(null, string.Join("\n", model.Lines));
            viewGroup.alpha = 0f;
            // ������Ч
            AudioManager.Instance.PlaySFX("gear");
            // ����warning
            StartCoroutine(PlayWarning());
        }

        private IEnumerator PlayWarning()
        {
            yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));
            yield return new WaitForSeconds(holdDuration);
            yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));
            // ���� view����������ʽ�Ի�
            view.Render(null, null);
            dialogueCtl.StartDialogue();
        }
        private IEnumerator Fade(float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                viewGroup.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            viewGroup.alpha = to;
        }
    }
}


