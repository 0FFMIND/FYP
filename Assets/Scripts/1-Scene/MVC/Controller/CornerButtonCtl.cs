using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using AudioSystem;
using TMPro;
using System;

namespace MVC
{
    [RequireComponent(typeof(Button))]
    public class CornerButtonCtl : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("�ĽǱ��")]
        public GameObject cornerTL;
        public GameObject cornerTR;
        public GameObject cornerBL;
        public GameObject cornerBR;
        [Header("���� & �ı� �л�")]
        public Image targetImage;          // ���뻻��ͼ�� Image
        public Sprite normalSprite;        // δ����ʱ����ͼ1
        public Sprite hoverSprite;         // ����ʱ����ͼ2
        [Header("���/ѡ������ʽ�л�")]
        public GameObject tickMark;      // ������ Tick
        public TextMeshProUGUI targetText; // ���� Image �µ� TextMeshProUGUI
        public Color normalTextColor = Color.black;
        public Color hoverTextColor = Color.white;
        //
        private Action<int> _onChoice;    // ���ⲿ����Ļص�
        private int _choiceId;            // �汾��ť��Ӧ�� targetNodeId
        private RectTransform rect;
        private Button btn;
        private ColorBlock defaultColors;
        private Sprite _originalSprite;
        [Header("�����������")]
        public float clickAnimDuration;     // ��ʱ��
        public float shrinkFactor;         // ��С����
        public float expandFactor;         // �������

        private void Awake()
        {
            // ��¼ԭʼ�����ڻָ�
            if (targetImage != null)
                _originalSprite = targetImage.sprite;
            btn = GetComponent<Button>();
            SetCorners(false);
        }
        public void SetupChoice(int choiceId, Action<int> onChoice)
        {
            _choiceId = choiceId;
            _onChoice = onChoice;
        }

        // ������룺��ʾ�Ľ�
        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFX("hover");
            SetCorners(true);
            // �л����� & ����
            if (targetImage != null && hoverSprite != null)
                targetImage.sprite = hoverSprite;
            if (targetText != null)
                targetText.color = hoverTextColor;
        }

        // ����Ƴ��������Ľ�
        public void OnPointerExit(PointerEventData eventData)
        {
            SetCorners(false);
            // �ָ����� & ����
            if (targetImage != null)
                targetImage.sprite = normalSprite ? normalSprite : _originalSprite;
            if (targetText != null)
                targetText.color = normalTextColor;
        }

        // ���ʱ����˸
        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFX("click");
            // SetCorners(false);
            // �ָ����� & ����
            if (targetImage != null)
                targetImage.sprite = normalSprite ? normalSprite : _originalSprite;
            if (targetText != null)
                targetText.color = normalTextColor;
            // �����������
            btn.interactable = false;
            StartCoroutine(ClickFeedbackCoroutine());
        }

        private IEnumerator ClickFeedbackCoroutine()
        {
            rect = GetComponent<RectTransform>();
            Vector3 original = rect.localScale;
            float half = clickAnimDuration / 2;

            // ��һ�룺���� shrinkFactor
            for (float t = 0; t < half; t += Time.unscaledDeltaTime)
            {
                float p = t / half;
                rect.localScale = Vector3.Lerp(original, original * shrinkFactor, p);
                yield return null;
            }
            rect.localScale = original * shrinkFactor;

            // �ڶ��룺�ŵ� expandFactor
            for (float t = 0; t < half; t += Time.unscaledDeltaTime)
            {
                float p = t / half;
                rect.localScale = Vector3.Lerp(original * shrinkFactor, original * expandFactor, p);
                yield return null;
            }
            rect.localScale = original * expandFactor;

            // ��һ�»ص�ԭʼ��С
            rect.localScale = original;

            // ���������󣬴����������Ļص�
            _onChoice?.Invoke(_choiceId);

            SetCorners(false);
            btn.interactable = true;
        }

        public void SetVisited(bool visited)
        {
            // ��� Button ��ͬһ�� GameObject �ϣ�
            btn = GetComponent<Button>();
            // ��� Button ��������������ĳ���ӽڵ��ϣ��������
            if (btn == null)
                btn = GetComponentInChildren<Button>();
            // �ȸ� Button.colors ��� normalColor
            var cb = btn.colors;
            if (visited)
            {
                cb.normalColor = Color.gray;
                cb.highlightedColor = Color.gray; // ����Ҳ����ͬɫ���������
                cb.pressedColor = Color.black;
                cb.selectedColor = Color.gray;
                btn.colors = cb;
            }
            // ����ʾ�����ع�
            tickMark.SetActive(visited);
        }

        private void SetCorners(bool on)
        {
            cornerTL.SetActive(on);
            cornerTR.SetActive(on);
            cornerBL.SetActive(on);
            cornerBR.SetActive(on);
        }

    }

}
