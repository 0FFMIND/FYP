using Manager;
using UnityEngine;

namespace MVC
{
    public class Scene1PlayerMoveAction : MonoBehaviour
    {
        [SerializeField]
        private float zoomInDistance;

        [SerializeField]
        private float zoomInDuration;

        [SerializeField]
        private float zoomOutDuration;

        [SerializeField]
        private CameraCtl cameraCtl;

        public void pan()
        {
            cameraCtl.PanTo(new Vector2(0f, 0f), 2f);
        }
        public void CameraZoomIn()
        {
            cameraCtl.ZoomOrthoBy(zoomInDistance, zoomInDuration);
        }

        public void CameraZoomOut()
        {
            cameraCtl.ZoomOrtho(5.3f, zoomOutDuration);
        }

        public void CameraShake()
        {
            AudioMgr.Instance.PlaySFX("Punch");
            cameraCtl.Shake();
        }
    }
}
