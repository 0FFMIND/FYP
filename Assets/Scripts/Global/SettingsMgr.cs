using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.SingletonPattern;

namespace Manager
{
    public class SettingsMgr : SingletonMB<SettingsMgr>
    {
        // 可被序列化
        [Serializable]
        public class SettingsDTO
        {
            public float bgmVolume = 1f;
            public float sfxVolume = 1f;
            public string language = "en";
            public List<PCMapping> keyBindings = new();
        }

        // AutoSingletonMB
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            EnsureCreated();
        }

    }
}


