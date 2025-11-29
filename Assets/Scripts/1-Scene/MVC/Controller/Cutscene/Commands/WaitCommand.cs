using System.Collections;
using UnityEngine;

namespace MVC
{
    public class WaitCommand : IScene1CutsceneCommand
    {
        private readonly float t;
        public WaitCommand(float t) { this.t = t; }
        public IEnumerator Execute(Scene1CutsceneContext ctx)
        {
            yield return new WaitForSeconds(t);
        }
    }
}