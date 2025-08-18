using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Utils
{
    public readonly struct EKeyRebindRequested
    {
        public readonly InputAction TargetAction;
        public EKeyRebindRequested(InputAction action) { TargetAction = action; }
    }
    public readonly struct EInputPressed
    {
        public readonly InputAction Action;

        public EInputPressed(InputAction action) => Action = action;
    }


    public struct ELanguageChanged
    {
        public string Language;

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
