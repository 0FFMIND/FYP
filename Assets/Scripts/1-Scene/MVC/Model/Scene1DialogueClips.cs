using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    public enum Scene1DialogueId
    {
        TimelineIntro,
        TimelineEnd,
        SignTutorialEnd,
        RooftopExploreIntro,
        VendingMachineEnd,
        RooftopExploreEnd,
        MeadowExploreIntro,
        MeadowExploreMid,
        MeadowExploreEnd,
        RunAwayIntro,
        RunAwayVoiceOver,
        RunAwayMid,
        RunAwayEnd
    }
    [Serializable]
    public class DialogueClip
    {
        public Scene1DialogueId dialogueId;
        public string textFile;
        public LineMapping[] mappings;
    }

    public class Scene1DialogueClips : MonoBehaviour
    {
        public List<DialogueClip> dialogMappings;

        /// <summary>
        /// 根据枚举 id 查找对应的 DialogueClip，没有则返回 null
        /// </summary>
        public DialogueClip GetClip(Scene1DialogueId id)
        {
            if (dialogMappings == null) return null;

            foreach (var clip in dialogMappings)
            {
                if (clip != null && clip.dialogueId == id)
                    return clip;
            }
            return null;
        }
    }
}