using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class StartGame : MonoBehaviour
{
    public void RestartGame()
    {
        SettingsMgr.Instance.ClearProgress();
        // ½øÈë1-Scene-UI
        EventBus.Publish(
            new ESceneFade(
                toScene: "1-Scene-UI",
                fadeOutDuration: 0.5f,
                fadeInDuration: 1f
            )
        );
    }
}
