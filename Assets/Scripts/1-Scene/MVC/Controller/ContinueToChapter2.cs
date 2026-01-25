using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ContinueToChapter2 : MonoBehaviour
{
    public void Continue()
    {
        // 切换到章节2场景
        EventBus.Publish(
            new ESceneFade(
                toScene: "2-Scene-UI",
                fadeOutDuration: 0.5f,
                fadeInDuration: 1f
            )
        );
    }
}
