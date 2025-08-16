using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private float parallaxFactor;

    private Vector3 lastCamPos;

    void Start()
    {
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;
        transform.position += delta * parallaxFactor;
        lastCamPos = cam.position;
    }
}
