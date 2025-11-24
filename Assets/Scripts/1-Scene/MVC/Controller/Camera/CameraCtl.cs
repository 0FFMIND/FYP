using UnityEngine;

public class CameraCtl : MonoBehaviour
{
    [Header("Sub modules")]
    [SerializeField]
    private CameraFollow follow;

    [SerializeField]
    private CameraShake shake;

    [SerializeField]
    private CameraMove move;

    private void Awake()
    {
        // 允许不手动拖引用：自动 GetComponent
        if (follow == null)
        {
            follow = GetComponent<CameraFollow>();
        }
        if (shake == null)
        {
            shake = GetComponent<CameraShake>();
        }
        if (move == null)
        {
            move = GetComponent<CameraMove>();
        }
    }

    // ===== Follow / Detach =====

    public void FollowPlayer()
    {
        if (follow == null)
        {
            return;
        }
        follow.ReattachToPlayer();
    }

    public void Detach()
    {
        if (follow == null)
        {
            return;
        }
        follow.DetachCamera();
    }

    // ===== Shake =====
    public void Shake(
        float durationOverride = -1f,
        float magnitudeOverride = -1f,
        float rotationOverride = -1f
    )
    {
        if (shake == null)
        {
            return;
        }
        shake.StartShaking(durationOverride, magnitudeOverride, rotationOverride);
    }

    // ===== Pan =====
    public void PanTo(Vector2 xy, float duration)
    {
        if (follow == null || move == null)
        {
            return;
        }
        // 确保已脱离人物，交给锚点驱动
        follow.DetachCamera();
        move.PanTo(xy, duration);
    }

    public void PanToY(float y, float duration)
    {
        if (follow == null || move == null)
        {
            return;
        }
        // 确保已脱离人物，交给锚点驱动
        follow.DetachCamera();
        move.PanToY(y, duration);
    }

    // ===== Zoom =====
    public void ZoomOrtho(float targetSize, float duration)
    {
        if (move == null)
        {
            return;
        }
        move.ZoomOrtho(targetSize, duration);
    }

    public void ZoomOrthoBy(float delta, float duration)
    {
        if (move == null)
        {
            return;
        }
        move.ZoomOrthoBy(delta, duration);
    }
}
