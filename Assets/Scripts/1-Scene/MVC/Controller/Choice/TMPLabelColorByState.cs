using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TMPLabelColorByState
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        ISelectHandler,
        IDeselectHandler
{
    [SerializeField]
    private TMP_Text label; // 子文本（不填则自动找）

    [Header("Colors")]
    [SerializeField]
    private Color normal = Color.white;

    [SerializeField]
    private Color highlighted = Color.black;

    [SerializeField]
    private Color pressed = Color.black;

    [SerializeField]
    private Color disabled = new Color(1f, 1f, 1f, 0.5f);

    private Button _btn;
    private bool _hover;
    private bool _isPressed;
    private bool _lastInteractable;

    void Awake()
    {
        _btn = GetComponent<Button>();
        if (!label)
            label = GetComponentInChildren<TMP_Text>(true);
    }

    void OnEnable()
    {
        _hover = _isPressed = false;
        _lastInteractable = _btn && _btn.interactable;
        Apply();
    }

    void Update()
    {
        // 运行时 interactable 变化时同步颜色
        if (_btn && _btn.interactable != _lastInteractable)
        {
            _lastInteractable = _btn.interactable;
            Apply();
        }
    }

    void OnDisable()
    {
        if (label)
            label.color = normal;
    }

    private void Apply()
    {
        if (!label)
            return;
        if (!_btn || !_btn.interactable)
        {
            label.color = disabled;
            return;
        }
        if (_isPressed)
        {
            label.color = pressed;
            return;
        }
        if (_hover)
        {
            label.color = highlighted;
            return;
        }
        label.color = normal;
    }

    public void OnPointerEnter(PointerEventData e)
    {
        _hover = true;
        Apply();
    }

    public void OnPointerExit(PointerEventData e)
    {
        _hover = false;
        _isPressed = false;
        Apply();
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Left)
        {
            _isPressed = true;
            Apply();
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        _isPressed = false;
        Apply();
    }

    public void OnSelect(BaseEventData e)
    {
        _hover = true;
        Apply();
    } // 键盘/手柄导航的高亮

    public void OnDeselect(BaseEventData e)
    {
        _hover = false;
        _isPressed = false;
        Apply();
    }
}
