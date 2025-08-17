using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public struct ELanguageChanged
    {
        public string Language;
        public ELanguageChanged(string lang) { Language = lang; }
    }
    public readonly struct EPauseChanged
    {
        public readonly bool IsPaused;
        public EPauseChanged(bool paused) => IsPaused = paused;
    }
    public readonly struct ESettingsChanged
    {
        public readonly SettingsDTO Settings;
        public ESettingsChanged(SettingsDTO settings) { Settings = settings; }
    }
}
