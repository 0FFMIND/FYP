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
    public class Scene1DialogueClip : DialogueClipBase
    {
        [Tooltip("Scene1 专用枚举 ID")]
        public Scene1DialogueId dialogueId;
    }

    public class Scene1DialogueClips : MonoBehaviour, IDialogueClipProvider
    {
        public List<Scene1DialogueClip> dialogMappings;
        /// <summary>
        /// 通过 int ID 查找 clip（给 TimelineDialogCtl 等通用调用方用）
        /// </summary>
        public DialogueClipBase GetClip(int id)
        {
            if (dialogMappings == null) return null;

            var enumId = (Scene1DialogueId)id;

            foreach (var clip in dialogMappings)
            {
                if (clip != null && clip.dialogueId == enumId)
                    return clip;  // 向上转型为 DialogueClipBase
            }
            return null;
        }
    }
}