using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float followYOffset = -0.5f;

    [SerializeField]
    private CinemachineVirtualCamera playerVCam;

    private Transform manualAnchor;
    private Transform player;
    private CinemachineTransposer transposer;
    private CinemachineFramingTransposer framingTransposer;
    private Vector3 baseFollowOffset;
    private Vector3 baseTrackedOffset;

    private void Awake()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        player = p != null ? p.transform : null;

        playerVCam.Follow = player.transform;
        transposer = playerVCam.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            baseFollowOffset = transposer.m_FollowOffset;
            transposer.m_FollowOffset = baseFollowOffset + Vector3.down * followYOffset;
        }

        framingTransposer = playerVCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framingTransposer != null)
        {
            baseTrackedOffset = framingTransposer.m_TrackedObjectOffset;
            framingTransposer.m_TrackedObjectOffset =
                baseTrackedOffset + Vector3.up * followYOffset;
        }
    }

    // 取消跟随玩家
    public void DetachCamera()
    {
        if (manualAnchor == null)
        {
            manualAnchor = new GameObject("CamManualAnchor").transform;
            manualAnchor.position = playerVCam.transform.position;
            manualAnchor.rotation = playerVCam.transform.rotation;
        }
        playerVCam.Follow = manualAnchor;
    }

    // 重新跟随玩家
    public void ReattachToPlayer()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            player = p != null ? p.transform : null;
        }
        if (player != null)
        {
            playerVCam.Follow = player;
        }
    }
}
