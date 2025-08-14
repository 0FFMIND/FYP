using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace MVC
{
    public class DialogueView : MonoBehaviour
    {
        [Tooltip("用于显示对话人物立绘的 Image 组件")]
        public Image img;
        [Tooltip("用于显示对话内容的 TextMeshProUGUI 组件")]
        public TextMeshProUGUI tmp;
        [SerializeField, Tooltip("图片淡入时长（秒），0 表示直接显示，无过渡效果")]
        private float imageFadeDuration;

        private Coroutine imageFadeCoroutine;
        private CanvasGroup imgGroup;

        private void Awake()
        {
            if (img != null)
            {
                imgGroup = img.GetComponent<CanvasGroup>();
                if (imgGroup == null)
                    imgGroup = img.gameObject.AddComponent<CanvasGroup>();
                img.gameObject.SetActive(false);
            }
        }

        private void FadeIn()
        {
            // 如果淡入时间 <= 0，直接显示，不做动画
            if (imageFadeDuration <= 0f)
            {
                imgGroup.alpha = 1f;
                imageFadeCoroutine = null;
                return;
            }
            if (imageFadeCoroutine != null)
                StopCoroutine(imageFadeCoroutine);
            imgGroup.alpha = 0f;
            imageFadeCoroutine = StartCoroutine(FadeInImage());
        }

        private IEnumerator FadeInImage()
        {
            float t = 0f;
            while (t < imageFadeDuration)
            {
                t += Time.deltaTime;
                imgGroup.alpha = Mathf.Lerp(0f, 1f, t / imageFadeDuration);
                yield return null;
            }
            imgGroup.alpha = 1f;
            imageFadeCoroutine = null;
        }

        public void Render(Sprite sprite, string text)
        {
            if(sprite == null)
            {
                if (img != null && img.gameObject.activeSelf)
                {
                    img.gameObject.SetActive(false);
                }
            }
            else
            {
                if (img != null && !img.gameObject.activeSelf)
                {
                    img.gameObject.SetActive(true);
                    FadeIn();
                }
                if(img != null && img.sprite != sprite)
                {
                    img.sprite = sprite;
                    FadeIn();
                }
            }
            tmp.text = text;
        }
    }
}


