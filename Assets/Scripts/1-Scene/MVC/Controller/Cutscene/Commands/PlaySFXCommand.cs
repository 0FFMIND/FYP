using System.Collections;
using Manager;
using UnityEngine;

namespace MVC
{
    public class PlaySFXCommand : IScene1CutsceneCommand
    {
        private readonly string sfxName;
        public PlaySFXCommand(string sfxName) { this.sfxName = sfxName; }
        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            AudioMgr.Instance.PlaySFX(sfxName);
            yield break;
        }
    }
}