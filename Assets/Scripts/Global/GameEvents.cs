using Manager;
using MVC;
using UnityEngine;

namespace Utils
{
    public readonly struct EJournalUIChanged { }

    public readonly struct EJournalStatusChanged
    {
        public readonly string Key;
        public readonly JournalStatus NewStatus;

        public EJournalStatusChanged(string key, JournalStatus newStatus)
        {
            Key = key;
            NewStatus = newStatus;
        }
    }

    public readonly struct EJournalStepChanged
    {
        public readonly string Key; // Journal 条目的 key
        public readonly int Index; // 实际命中的 contents 索引
        public readonly StepState State;

        public EJournalStepChanged(string key, int contentIndex, StepState newState)
        {
            Key = key;
            Index = contentIndex;
            State = newState;
        }
    }

    public readonly struct EJournalProgressChanged
    {
        public readonly string Key;
        public readonly int Done;
        public readonly int Total;

        public EJournalProgressChanged(string key, int done, int total)
        {
            Key = key;
            Done = done;
            Total = total;
        }
    }

    public readonly struct EJournalSelected
    {
        public readonly string Key;

        public EJournalSelected(string key)
        {
            Key = key;
        }
    }

    public readonly struct EScene1ArrivalPhaseChange
    {
        public readonly Scene1Phase Phase;

        public EScene1ArrivalPhaseChange(Scene1Phase phase) => Phase = phase;
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

    public readonly struct ESceneFade
    {
        public readonly string ToScene;
        public readonly float FadeOutDuration;
        public readonly float FadeInDuration;

        public ESceneFade(
            string toScene,
            float fadeOutDuration,
            float fadeInDuration
        ) =>
            (ToScene, FadeOutDuration, FadeInDuration) = (
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
