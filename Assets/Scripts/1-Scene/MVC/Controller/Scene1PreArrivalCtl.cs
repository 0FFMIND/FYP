using System.Collections;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class Scene1PreArrivalCtl : MonoBehaviour
    {
        [SerializeField]
        private GameObject warningViewCH;
        [SerializeField]
        private GameObject warningViewEN;

        private GameObject warningView;

        // 1-Scene-warning.txt
        [SerializeField]
        private string warningTxt;

        // 对话控制器
        [SerializeField]
        private Scene1PreArrivalDialogCtl dialogueCtl;

        // 这里canvasgroup用来调整不透明度
        private CanvasGroup viewGroup;

        [SerializeField]
        private float fadeInDuration;

        [SerializeField]
        private float fadeOutDuration;

        [SerializeField, Tooltip("停留时长（秒）")]
        private float holdDuration;

        private IEnumerator Start()
        {
            if(SettingsMgr.Instance.GetLanguage() == LanguageCode.zh)
            {
                warningView = warningViewCH;
            }else if(SettingsMgr.Instance.GetLanguage() == LanguageCode.en)
            {
                warningView = warningViewEN;
            }
            viewGroup = warningView.GetComponent<CanvasGroup>();
            // 关闭暂停菜单
            PauseMgr.Instance.SetPauseEnabled(false);
            // 关闭dialogCtl
            dialogueCtl.HideDialogue();
            // warningView
            warningView.SetActive(true);
            // 整体wanringView的淡入淡出
            viewGroup.alpha = 0f;
            // 加载音效
            AudioManager.Instance.PlaySFX("gear");
            // 播放warning
            yield return StartCoroutine(PlayWarning());
            // 隐藏warningView
            warningView.SetActive(false);
            // FadeIn
            TransitionMgr.Instance.FadeIn(0.5f);
            // 进入对话控制器
            dialogueCtl.StartDialogue();
            // 更新日记
            EventBus.Publish(new EJournalStatusChanged("reachRooftop", JournalStatus.Active));
            // 开启暂停菜单
            PauseMgr.Instance.SetPauseEnabled(true);
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
