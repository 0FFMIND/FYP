using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utils;

public class BackToTitle : MonoBehaviour
{
    public void BackTitle()
    {
        // 切换到标题场景
        EventBus.Publish(
            new ESceneFade(
                toScene: "Title-Scene",
                fadeOutDuration: 0.5f,
                fadeInDuration: 1f
            )
        );
    }
    public void BackTitleAndClosePauseMenu()
    {
        // 关闭菜单
        PauseMgr.Instance.TogglePause();
        // 切换到标题场景
        EventBus.Publish(
            new ESceneFade(
                toScene: "Title-Scene",
                fadeOutDuration: 0.5f,
                fadeInDuration: 1f
            )
        );
    }
}
