using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    [SerializeField]
    private Transform cam;

    [SerializeField]
    private float parallaxFactor;

    [SerializeField]
    public bool isOn = true;

    private Vector3 lastCamPos;

    void Start()
    {
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        if (isOn)
        {
            Vector3 delta = cam.position - lastCamPos;
            transform.position += delta * parallaxFactor;
            lastCamPos = cam.position;
        }
    }
}
