using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if TMP_PRESENT
using TMPro;
#endif

public class TitleButtonText : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler, IPointerClickHandler
{
    private Graphic uiText;

    [Header("Colors")]
    [SerializeField] private Color normal = Color.black;
    [SerializeField] private Color highlighted = Color.white;
    [SerializeField] private Color pressed = Color.yellow;
    [SerializeField] private Color disabled = Color.white;

    private Button btn;
    private bool isHover;
    private bool isPressed;
    private bool isSelected;

    private void Awake()
    {
        btn = GetComponent<Button>();
        uiText = GetComponentInChildren<TextMeshProUGUI>(true);
        Apply();
    }

    private void OnEnable()
    {
        // 每次打开都从“初始态”开始
        isHover = false;
        isPressed = false;
        isSelected = false;
        Apply();
    }

    private void OnDisable()
    {
        // 关闭时把 EventSystem 的选中也清掉（避免下次打开仍是 Selected）
        if (EventSystem.current != null)
        {
            var cur = EventSystem.current.currentSelectedGameObject;
            if (cur != null && cur.transform.IsChildOf(transform))
                EventSystem.current.SetSelectedGameObject(null);
        }

        // 关闭时也重置，避免没收到 Exit/Deselect 回调
        isHover = false;
        isPressed = false;
        isSelected = false;
    }

    private void Apply()
    {
        if (uiText == null) return;

        bool interactable = (btn == null) || btn.interactable;
        if (!interactable)
        {
            uiText.color = disabled;
            return;
        }

        if (isPressed) uiText.color = pressed;
        else if (isHover || isSelected) uiText.color = highlighted;
        else uiText.color = normal;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        isSelected = false;
        Apply();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
        Apply();
    }
    public void OnPointerExit(PointerEventData eventData) { isHover = false; isPressed = false; Apply(); }

    public void OnPointerDown(PointerEventData eventData) { isPressed = true; Apply(); }
    public void OnPointerUp(PointerEventData eventData)
    {
        AudioMgr.Instance.PlaySFX("buttonClick");
        isPressed = false;
        Apply();
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        Apply();
    }
    public void OnDeselect(BaseEventData eventData) { isSelected = false; isPressed = false; Apply(); }
}
