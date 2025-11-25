using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace MVC
{
    public class ArrowIndicator : MonoBehaviour
    {
        [Header("翻页箭头位移")]
        [SerializeField]
        protected float arrowOffset = 0.4f; // 首次定位的像素偏移

        [SerializeField]
        protected int downFrames = 100; // 向下移动时等待帧数

        [SerializeField]
        protected float downDistance = 0.07f; // 向下移动的世界/本地单位

        [SerializeField]
        protected int upFrames = 100; // 向上移动时等待帧数

        private Transform arrow;
        private SpriteRenderer arrowSr;
        private Coroutine bounceCoroutine;
        private Vector3 arrowOriginalScale;


        // 确保箭头实例已创建并挂到指定父物体下
        public void EnsureCreated(Transform parent)
        {
            if (arrow != null)
            {
                return;
            }
            var prefab = Resources.Load<GameObject>("Prefabs/1-Scene/DownRow");
            if (!prefab)
            {
                Debug.LogError($"[ArrowIndicator] Resources.Load 失败");
                return;
            }

            // 实例化
            var go = Instantiate(prefab, parent);

            // 缓存实例的 Transform，后续用于定位与移动
            arrow = go.transform;
            arrowOriginalScale = arrow.localScale;
            arrowSr = go.GetComponent<SpriteRenderer>();
            // 初始隐藏
            arrow.gameObject.SetActive(false);
        }

        public void SetColor(Color c)
        {
            if (arrowSr != null)
                arrowSr.color = c;
        }

        public void SetArrowScale(float factor)
        {
            if (arrow == null) return;
            arrow.localScale = arrowOriginalScale * factor;
        }

        public void PositionArrowUnderText(TMP_Text tmp)
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
                return;
            // 强制刷新 TMP 网格与 bounds，确保 textBounds 为最新
            tmp.ForceMeshUpdate();
            Bounds b = tmp.textBounds;
            Vector3 localBotCenter = new Vector3(b.center.x, b.min.y, 0);
            Vector3 worldBotCenter = tmp.transform.TransformPoint(localBotCenter);
            Vector3 downOffset = Vector3.down * arrowOffset;
            arrow.position = new Vector3(
                worldBotCenter.x,
                worldBotCenter.y + downOffset.y,
                arrow.position.z
            );
            // 显示，并向下偏移
            arrow.gameObject.SetActive(true);
            // 启动抖动
            if (bounceCoroutine != null)
            {
                StopCoroutine(bounceCoroutine);
            }
            bounceCoroutine = StartCoroutine(ArrowBounce());
        }

        public bool IsActive()
        {
            return arrow != null && arrow.gameObject.activeSelf;
        }

        // 隐藏箭头并停止抖动
        public void Hide()
        {
            if (arrow == null) return;
            arrow.gameObject.SetActive(false);
            if (bounceCoroutine != null)
            {
                StopCoroutine(bounceCoroutine);
                bounceCoroutine = null;
            }
        }

        // 箭头上下抖动协程
        private IEnumerator ArrowBounce()
        {
            // 记录原始位置
            Vector3 original = arrow.position;
            Vector3 target = original + Vector3.down * downDistance;
            while (true)
            {
                // 平滑下移
                for (int i = 0; i <= downFrames; i++)
                {
                    float t = i / (float)downFrames; // 从 0 到 1
                    arrow.position = Vector3.Lerp(original, target, t);
                    yield return null;
                }
                // 平滑上移
                for (int i = 0; i <= upFrames; i++)
                {
                    float t = i / (float)upFrames;
                    arrow.position = Vector3.Lerp(target, original, t);
                    yield return null;
                }
            }
        }
    }
}