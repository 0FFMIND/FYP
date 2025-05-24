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
        // 对话控制器，warning播放完进入
        [SerializeField]
        private DialogueCtl dialogueCtl;
        //
        private DialogueModel model;
        // 这里canvasgroup用来调整不透明度
        private CanvasGroup viewGroup;
        [SerializeField]
        private float fadeInDuration;
        [SerializeField]
        private float fadeOutDuration;
        [SerializeField, Tooltip("停留时长（秒）")]
        private float holdDuration;
        private void Awake()
        {
            viewGroup = view.GetComponent<CanvasGroup>();
        }
        void Start()
        {
            choiceCanvas.gameObject.SetActive(false);
            // 读取text"1-Scene-warning.txt"
            model = new DialogueModel(dialogueTxt);
            dialogueCtl.arrow.gameObject.SetActive(false);
            dialogueCtl.view.Render(null, null);
            view.Render(null, string.Join("\n", model.Lines));
            viewGroup.alpha = 0f;
            // 加载音效
            AudioManager.Instance.PlaySFX("gear");
            // 播放warning
            StartCoroutine(PlayWarning());
        }

        private IEnumerator PlayWarning()
        {
            yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));
            yield return new WaitForSeconds(holdDuration);
            yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));
            // 隐藏 view，并启动正式对话
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


