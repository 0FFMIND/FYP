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
        public Image img;
        public TextMeshProUGUI tmp;
        [SerializeField, Tooltip("����ʱ�����룩")]
        private float imageFadeDuration = 0.3f;

        // �ڲ�����׷�ٵ�ǰ�ĵ���Э��
        private Coroutine imageFadeCoroutine;
        // ��������alphaֵ
        private CanvasGroup imgGroup;

        [Header("ѡ�ť����")]
        public RectTransform choicesContainer;   // �� VerticalLayoutGroup
        public GameObject choiceHeaderPrefab;    // ͷ����ʾ
        public Button choiceButtonPrefab;        // �� TMP Text �İ�ťԤ��


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
                // disabled
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
        public void ShowChoices(HashSet<int> visited, string choicesTxt, Choice[] choices, UnityAction<int> onChoiceSelected)
        {
            // ��վɰ�ť
            foreach (Transform t in choicesContainer)
                Destroy(t.gameObject);
            // ����header
            if(choicesTxt.Length > 0)
            {
                var go = Instantiate(choiceHeaderPrefab, choicesContainer);
                go.GetComponent<TextMeshProUGUI>().text = choicesTxt;
            }
            // �����°�ť
            foreach (var choice in choices)
            {
                var btn = Instantiate(choiceButtonPrefab, choicesContainer);
                // �õ����ƽű�
                var ctl = btn.GetComponent<CornerButtonCtl>();

                // 2) ����Ѿ� visited��������һ�����ͻһ�����ʾ�Թ�
                if (visited.Contains(choice.targetNodeId))
                    ctl.SetVisited(true);
                else
                    ctl.SetVisited(false);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                btn.onClick.AddListener(() => {
                    onChoiceSelected(choice.targetNodeId);
                });
            }
            choicesContainer.gameObject.SetActive(true);
        }
        public void HideChoices()
        {
            choicesContainer.gameObject.SetActive(false);
        }

    }
}


