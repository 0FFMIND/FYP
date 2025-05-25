using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using AudioSystem;

[RequireComponent(typeof(Button))]
public class CornerButtonCtl : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // 拖四个角到这
    public GameObject cornerTL;
    public GameObject cornerTR;
    public GameObject cornerBL;
    public GameObject cornerBR;

    // 闪烁用
    public float flashDuration = 0.2f;
    public int flashCount = 3;

    private Graphic targetGraphic;  // 按钮背景或文本

    private void Awake()
    {
        SetCorners(false);
    }

    // 鼠标移入：显示四角
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("hover");
        SetCorners(true);
    }

    // 鼠标移出：隐藏四角
    public void OnPointerExit(PointerEventData eventData)
    {
        SetCorners(false);
    }

    // 点击时：闪烁
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("click");
        SetCorners(false);
    }

    private void SetCorners(bool on)
    {
        cornerTL.SetActive(on);
        cornerTR.SetActive(on);
        cornerBL.SetActive(on);
        cornerBR.SetActive(on);
    }

}
