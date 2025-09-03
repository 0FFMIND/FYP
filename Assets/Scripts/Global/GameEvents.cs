using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Utils
{
    public readonly struct EKeyRebind
    {
        public readonly InputAction Action;
        public EKeyRebind(InputAction action) { Action = action; }
    }

    public readonly struct EInputPressed
    {
        public readonly InputAction Action;

        public EInputPressed(InputAction action) => Action = action;
    }

    public readonly struct ELanguageChanged
    {
        public readonly string Language;

        public ELanguageChanged(string lang) => Language = lang;
    }

    public readonly struct EPauseChanged
    {
        public readonly bool IsPaused;

        public EPauseChanged(bool paused) => IsPaused = paused;
    }

    public readonly struct ESettingsChanged
    {
        public readonly SettingsDTO Settings;

        public ESettingsChanged(SettingsDTO settings) => Settings = settings;
    }
}
