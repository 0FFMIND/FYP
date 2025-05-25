using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using AudioSystem;

[RequireComponent(typeof(Button))]
public class CornerButtonCtl : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // ���ĸ��ǵ���
    public GameObject cornerTL;
    public GameObject cornerTR;
    public GameObject cornerBL;
    public GameObject cornerBR;

    // ��˸��
    public float flashDuration = 0.2f;
    public int flashCount = 3;

    private Graphic targetGraphic;  // ��ť�������ı�

    private void Awake()
    {
        SetCorners(false);
    }

    // ������룺��ʾ�Ľ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("hover");
        SetCorners(true);
    }

    // ����Ƴ��������Ľ�
    public void OnPointerExit(PointerEventData eventData)
    {
        SetCorners(false);
    }

    // ���ʱ����˸
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
