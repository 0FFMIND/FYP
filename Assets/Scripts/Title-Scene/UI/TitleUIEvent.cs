using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class TitleUIEvent : MonoBehaviour
{
    public ButtonsReveal buttonsReveal;
    public void PlayTitleBGM()
    {
        AudioMgr.Instance.PlayBGM("0-bgm");
    }
    public void PlayButtonsAnim()
    {
        buttonsReveal.PlayButton();
    }
}
