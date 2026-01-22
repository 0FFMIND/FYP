using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    /// <summary>
    /// 玩家跑向门这一小段过场的命令序列
    /// </summary>
    public class Scene1RunToDoor
    {

        public IEnumerable<IScene1CutsceneCommand> Build()
        {

            yield return new ContextActionCommand(ctx =>
            {
                ctx.Flower.SetActive(false);
                ctx.Door.SetActive(false);
                ctx.Mover.SetFace(Direction.Down);
                ctx.CameraCtl.ZoomOrthoBy(0.7f, 0.2f);
            });
            yield return new WaitCommand(0.5f);
            yield return new ContextActionCommand(ctx =>
            {
                var target = new Vector3(ctx.Mover.transform.position.x, 0.4f, ctx.Mover.transform.position.z);
                ctx.Mover.StartMove(target, 6f, Direction.Down, null);
            });
            yield return new WaitCommand(0.85f);
            yield return new SetFaceCommand(Direction.Right);
            yield return new WaitCommand(0.15f);
            yield return new ContextActionCommand(ctx =>
            {
                var target = new Vector3(6.88f, ctx.Mover.transform.position.y, ctx.Mover.transform.position.z);
                ctx.Mover.StartMove(target, 6f, Direction.Right, null);
            });
            yield return new WaitCommand(3.5f);
            yield return new SetFaceCommand(Direction.Up);
            yield return new WaitCommand(0.2f);
            yield return new ContextActionCommand(ctx =>
            {
                var target = new Vector3(ctx.Mover.transform.position.x, 2.13f, ctx.Mover.transform.position.z);
                ctx.Mover.StartMove(target, 6f, Direction.Up, null);
            });
            yield return new WaitCommand(1f);
            AudioMgr.Instance.PlaySFX("dooropen");
            yield return new ContextActionCommand(ctx =>
            {
                ctx.Mover.SetLock(true);
            });
            yield return new SetPlayerCloseDoorSpriteCommand(0);
            yield return new WaitCommand(0.15f);

            yield return new SetPlayerCloseDoorSpriteCommand(1);
            yield return new WaitCommand(0.15f);
            yield return new ContextActionCommand(ctx =>
           {
               ctx.Door.SetActive(false);
               ctx.DoorGO.SetActive(false);
               ctx.OpenDoor.SetActive(true);
           });
            yield return new SetPlayerCloseDoorSpriteCommand(0);
            yield return new WaitCommand(0.15f);

            yield return new SetPlayerCloseDoorSpriteCommand(2);


            yield return new WaitCommand(0.15f);

            yield return new ContextActionCommand(ctx =>
            {

                ctx.Switcher.EnterUI(() =>
                {
                    AudioMgr.Instance.PlayBGM("1-bgm-4", 0f);
                    ctx.UICtl.StartClipDialogue(Scene1DialogueId.RunAwayEnd, () =>
                    {
                        SettingsMgr.Instance.SetChapter1Completed(true);
                        AudioMgr.Instance.StopBGM();
                        // 切换到过度场景
                        EventBus.Publish(
                            new ESceneFade(
                                toScene: "Title-Scene",
                                fadeOutDuration: 0.5f,
                                fadeInDuration: 1f
                            )
                        );
                    });
                });
            });

        }
    }
}
