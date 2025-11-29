using Manager;
using UnityEngine;

public class Scene1MeadowCtl : MonoBehaviour
{
    public void PlayBell()
    {
        AudioMgr.Instance.StopBGM();
        AudioMgr.Instance.PlaySFX("schoolBell");
    }
    public void PlayRunning()
    {
        AudioMgr.Instance.StopSFXImmediate();
        AudioMgr.Instance.PlayBGM("1-bgm-3");
    }
}
