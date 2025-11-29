using System.Collections;
using UnityEngine;

namespace MVC
{
    public class LockPlayerCommand : IScene1CutsceneCommand
    {
        private readonly bool lockState;
        public LockPlayerCommand(bool lockState) { this.lockState = lockState; }
        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            ctx.Mover.SetLock(lockState);
            yield break;
        }
    }
}