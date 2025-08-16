using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 玩家
    public float smoothSpeed = 0.15f; // 缓动系数

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null)
            return;

        // 目标位置
        Vector3 desiredPosition = target.position;
        Vector3 smooth = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothSpeed
        );
        // 平滑移动
        transform.position = new Vector3(smooth.x, smooth.y, 0f);
    }
}
