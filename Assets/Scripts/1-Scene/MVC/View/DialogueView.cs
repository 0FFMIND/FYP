using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

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
        // 用来调整alpha值
        private CanvasGroup imgGroup;

        [Header("选项按钮容器")]
        public RectTransform choicesContainer;   // 挂 VerticalLayoutGroup
        public GameObject choiceHeaderPrefab;    // 头顶提示
        public Button choiceButtonPrefab;        // 带 TMP Text 的按钮预制


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
        public void ShowChoices(string choicesTxt, Choice[] choices, UnityAction<int> onChoiceSelected)
        {
            // 清空旧按钮
            foreach (Transform t in choicesContainer)
                Destroy(t.gameObject);
            // 生成header
            if(choicesTxt.Length > 0)
            {
                var go = Instantiate(choiceHeaderPrefab, choicesContainer);
                go.GetComponent<TextMeshProUGUI>().text = choicesTxt;
            }
            // 生成新按钮
            foreach (var choice in choices)
            {
                var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                btn.onClick.AddListener(() => onChoiceSelected(choice.targetNodeId));
            }
            choicesContainer.gameObject.SetActive(true);
        }
        public void HideChoices()
        {
            choicesContainer.gameObject.SetActive(false);
        }

    }
}


