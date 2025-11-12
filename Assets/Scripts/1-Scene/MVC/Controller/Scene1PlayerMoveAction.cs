using Manager;
using System.Collections;
using System.Collections.Generic;
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
        private CameraFollow cameraFollow;

        public void pan()
        {
            cameraFollow.PanTo(new Vector2(0f, 0f), 2f);
        }
        public void CameraZoomIn()
        {
            cameraFollow.ZoomOrthoBy(zoomInDistance, zoomInDuration);
        }

        public void CameraZoomOut()
        {
            cameraFollow.ZoomOrtho(5.0f, zoomOutDuration);
        }

        public void CameraShake()
        {
            AudioManager.Instance.PlaySFX("Punch");
            cameraFollow.StartShaking();
        }
    }
}
