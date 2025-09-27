using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    public class AudioSignals : MonoBehaviour
    {
        public void PlayBGM(string bgm) => Manager.AudioManager.Instance.PlayBGM(bgm);
        public void StopBGM() => Manager.AudioManager.Instance.StopBGM();
        public void PlaySFX(string sfx) => Manager.AudioManager.Instance.PlaySFX(sfx);
    }
}
