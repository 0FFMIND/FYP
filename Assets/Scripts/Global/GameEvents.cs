using Manager;
using MVC;
using UnityEngine;

namespace Utils
{
    public struct JournalAdvanceEvent
    {
        // 目标条目的唯一键，如 "interactboard"
        public readonly string Key;

        // 该状态对应的展示文案
        public readonly string Title;

        public JournalAdvanceEvent(string key, string title)
        {
            Key = key;
            Title = title;
        }
    }

    public readonly struct EScene1ArrivalStateChange
    {
        public readonly Scene1State State;

        public EScene1ArrivalStateChange(Scene1State state) => State = state;
    }

    public readonly struct EInteract
    {
        public readonly InteractModel Model;

        public EInteract(InteractModel model) => Model = model;
    }

    public readonly struct EInteractEnd { }

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
