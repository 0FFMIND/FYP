using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        var player = GameObject.FindGameObjectWithTag("Player");
        vcam.Follow = player.transform;
    }
}
