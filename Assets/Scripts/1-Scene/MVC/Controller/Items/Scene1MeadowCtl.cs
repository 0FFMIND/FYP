using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1MeadowCtl : MonoBehaviour
{
    public void PlayBell()
    {
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySFX("schoolBell");
    }
    public void PlayRunning()
    {
        AudioManager.Instance.StopSFXImmediate();
        AudioManager.Instance.PlayBGM("1-bgm-3");
    }
}
