using Manager;
using MVC;
using UnityEngine;

namespace Utils
{
    public readonly struct EKeySet
    {
        public readonly InputAction Action;
        public readonly KeyCode Key;

        public EKeySet(InputAction action, KeyCode key)
        {
            Action = action;
            Key = key;
        }
    }

    public readonly struct EVolumeSet
    {
        public readonly float Db;
        public readonly VolumeType Type;

        public EVolumeSet(float db, VolumeType type)
        {
            Type = type;
            Db = db;
        }
    }

    public readonly struct ESceneFadeAdditiveDisable
    {
        public readonly string FromScene;
        public readonly string ToScene;
        public readonly float FadeOutDuration;
        public readonly float FadeInDuration;

        public ESceneFadeAdditiveDisable(
            string fromScene,
            string toScene,
            float fadeOutDuration,
            float fadeInDuration
        ) =>
            (FromScene, ToScene, FadeOutDuration, FadeInDuration) = (
                fromScene,
                toScene,
                fadeOutDuration,
                fadeInDuration
            );
    }

    public readonly struct EInputPressed
    {
        public readonly InputAction Action;

        public EInputPressed(InputAction action) => Action = action;
    }

    public readonly struct EInputUnPressed
    {
        public readonly InputAction Action;

        public EInputUnPressed(InputAction action) => Action = action;
    }

    public readonly struct ELanguageSet
    {
        public readonly LanguageCode Language;

        public ELanguageSet(LanguageCode lang) => Language = lang;
    }

    public readonly struct ELanguageChanged
    {
        public readonly LanguageCode Language;

        public ELanguageChanged(LanguageCode lang) => Language = lang;
    }

    public readonly struct EPauseChanged
    {
        public readonly bool IsPaused;

        public EPauseChanged(bool paused) => IsPaused = paused;
    }

    public readonly struct ESettingsChanged
    {
        public readonly SettingsData Settings;

        public ESettingsChanged(SettingsData settings) => Settings = settings;
    }
}
