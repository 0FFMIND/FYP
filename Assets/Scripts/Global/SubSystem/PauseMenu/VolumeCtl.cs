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
            SetScrollBar();
        }

        private void SetScrollBar()
        {
            float bgmValue = AudioMgr.Instance.GetBGMVolumeNormalized();
            bgmScrollBar.SetValueWithoutNotify(bgmValue);
            float sfxValue = AudioMgr.Instance.GetSFXVolumeNormalized();
            sfxScrollBar.SetValueWithoutNotify(sfxValue);
            float mixerValue = AudioMgr.Instance.GetMixerVolumeNormalized();
            mixerScrollBar.SetValueWithoutNotify(mixerValue);
        }

        public void HandleResetDefaults()
        {
            var fields = new[]
            {
                SettingField.BgmVolume,
                SettingField.SfxVolume,
                SettingField.MixerVolume,
            };

            SettingsMgr.Instance.ResetToDefaults(fields);
            SetScrollBar();
        }

        public void HandleBGMVolumeChange()
        {
            // 当 BGM 音量滑条数值改变时，把最新值写回 AudioManager
            float value = bgmScrollBar.value;
            AudioMgr.Instance.SetBGMVolumeNormalized(value);
        }

        public void HandleSFXVolumeChange()
        {
            // 当 SFX 音量滑条数值改变时，把最新值写回 AudioManager
            float value = sfxScrollBar.value;
            AudioMgr.Instance.SetSFXVolumeNormalized(value);
        }

        public void HandleMixerVolumeChange()
        {
            // 当 Mixer 音量滑条数值改变时，把最新值写回 AudioManager
            float value = mixerScrollBar.value;
            AudioMgr.Instance.SetMixerVolumeNormalized(value);
        }
    }
}
