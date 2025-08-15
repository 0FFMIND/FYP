using System.Collections;
using System.Collections.Generic;
using AudioSystem;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public class VolumeCtl : MonoBehaviour
    {
        [SerializeField] private Scrollbar bgmScrollBar;
        private void OnEnable()
        {
            float value = AudioManager.Instance.GetBGMVolume();
            bgmScrollBar.SetValueWithoutNotify(value);
        }
        public void HandleBGMVolumeChange()
        {
            float value = bgmScrollBar.value;
            AudioManager.Instance.SetBGMVolume(value);
        }
    }
}
