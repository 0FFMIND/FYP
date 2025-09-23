using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public class VolumeCtl : MonoBehaviour
    {
        [SerializeField]
        private Scrollbar bgmScrollBar;

        [SerializeField]
        private Scrollbar sfxScrollBar;

        [SerializeField]
        private Scrollbar mixerScrollBar;

        private void OnEnable()
        {
            // 初始化音量滑条为当前音量值
            float bgmValue = AudioManager.Instance.GetBGMVolumeNormalized();
            bgmScrollBar.SetValueWithoutNotify(bgmValue);
            float sfxValue = AudioManager.Instance.GetSFXVolumeNormalized();
            sfxScrollBar.SetValueWithoutNotify(sfxValue);
            float mixerValue = AudioManager.Instance.GetMixerVolumeNormalized();
            mixerScrollBar.SetValueWithoutNotify(mixerValue);
        }

        public void HandleBGMVolumeChange()
        {
            // 当 BGM 音量滑条数值改变时，把最新值写回 AudioManager
            float value = bgmScrollBar.value;
            AudioManager.Instance.SetBGMVolumeNormalized(value);
        }

        public void HandleSFXVolumeChange()
        {
            // 当 SFX 音量滑条数值改变时，把最新值写回 AudioManager
            float value = sfxScrollBar.value;
            AudioManager.Instance.SetSFXVolumeNormalized(value);
        }

        public void HandleMixerVolumeChange()
        {
            // 当 Mixer 音量滑条数值改变时，把最新值写回 AudioManager
            float value = mixerScrollBar.value;
            AudioManager.Instance.SetMixerVolumeNormalized(value);
        }
    }
}
