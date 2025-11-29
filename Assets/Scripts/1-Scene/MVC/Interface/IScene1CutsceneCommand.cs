using System.Collections;
namespace MVC
{
    public interface IScene1CutsceneCommand
    {
        public IEnumerator Execute(Scene1CutsceneContext ctx);
    }
}
