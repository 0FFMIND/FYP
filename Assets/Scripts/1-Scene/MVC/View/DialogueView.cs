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
        [SerializeField, Tooltip("����ʱ�����룩")]
        private float imageFadeDuration = 0.3f;

        // �ڲ�����׷�ٵ�ǰ�ĵ���Э��
        private Coroutine imageFadeCoroutine;

        // ȷ�� CanvasGroup �� img ��
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


