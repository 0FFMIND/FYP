using System.Collections;
using System.Collections.Generic;
using AudioSystem;
using UnityEngine;

namespace MVC
{
    public class Scene1Ctl : MonoBehaviour
    {
        [SerializeField]
        private DialogueView warningView;

        // 1-Scene-warning.txt
        [SerializeField]
        private string warningTxt;

        // 对话控制器
        [SerializeField]
        private Scene1DialogueCtl dialogueCtl;

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
            viewGroup = warningView.GetComponent<CanvasGroup>();
        }

        private IEnumerator Start()
        {
            // 关闭dialogCtl
            dialogueCtl.HideDialogue();
            // 将"1-Scene-warning.txt"放到warningView中
            warningView.Render(null, string.Join("\n", new DialogueModel(warningTxt).Lines));
            // 整体wanringView的淡入淡出
            viewGroup.alpha = 0f;
            // 加载音效
            AudioManager.Instance.PlaySFX("gear");
            // 播放warning
            yield return StartCoroutine(PlayWarning());
            // 隐藏warningView
            warningView.Render(null, null);
            // 进入对话控制器
            dialogueCtl.StartDialogue();
        }

        private IEnumerator PlayWarning()
        {
            yield return StartCoroutine(Fade(0f, 1f, fadeInDuration));
            yield return new WaitForSeconds(holdDuration);
            yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));
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
