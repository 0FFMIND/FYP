using System.Collections.Generic;

namespace MVC
{
    /// <summary>
    /// 玩家关门这一小段过场的命令序列
    /// 只负责构造命令列表，不关心如何执行
    /// </summary>
    public class Scene1CloseDoor
    {

        public IEnumerable<IScene1CutsceneCommand> Build()
        {
            yield return new LockPlayerCommand(true);
            yield return new PlaySFXCommand("dooropen");

            yield return new SetPlayerCloseDoorSpriteCommand(0);
            yield return new WaitCommand(0.15f);

            yield return new SetPlayerCloseDoorSpriteCommand(1);
            yield return new WaitCommand(0.15f);

            yield return new SetPlayerCloseDoorSpriteCommand(0);
            yield return new WaitCommand(0.15f);

            yield return new SetPlayerCloseDoorSpriteCommand(2);
            yield return new WaitCommand(0.15f);

            yield return new LockPlayerCommand(false);
        }
    }
}
