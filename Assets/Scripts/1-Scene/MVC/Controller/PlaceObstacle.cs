using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MVC
{
    public class PlaceObstacle : MonoBehaviour
    {
        private SpriteRenderer top; // 指向要“压住人”的那张Renderer（上沿/整张）
        private Transform player; // 指向玩家
        private SpritesOutline outline;
        public float xMargin = 0.1f; // 额外容差（世界单位）
        public int baseOrder = 0; // 与玩家相同的 Order（让Y排序接管）
        public int sideOrder = -1; // 站侧面时强制在玩家后面

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            top = GetComponent<SpriteRenderer>();
            outline = GetComponent<SpritesOutline>();
        }

        void LateUpdate()
        {
            if (!top || !player)
                return;
            var cx = top.bounds.center.x;
            var half = top.bounds.extents.x;
            bool xOverlap = Mathf.Abs(player.position.x - cx) <= (half + xMargin);
            top.sortingOrder = xOverlap ? baseOrder : sideOrder;
            outline.ChangeOrder(top.sortingOrder);
        }
    }

}
