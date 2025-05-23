using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

namespace MVC
{
    public class DialogueView : MonoBehaviour
    {
        public Image img;
        public TextMeshProUGUI tmp;
        [SerializeField, Tooltip("淡入时长（秒）")]
        private float imageFadeDuration = 0.3f;

        // 内部用来追踪当前的淡入协程
        private Coroutine imageFadeCoroutine;

        // 确保 CanvasGroup 在 img 上
        private CanvasGroup imgGroup;
        private void Awake()
        {
            if (img != null)
            {
                imgGroup = img.GetComponent<CanvasGroup>();
                if (imgGroup == null)
                    imgGroup = img.gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void FadeIn()
        {
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
                if (img != null && img.enabled)
                {
                    img.enabled = false;
                }
            }
            else
            {
                // disabled
                if (img != null && !img.enabled)
                {
                    img.enabled = true;
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


