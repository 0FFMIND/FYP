using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace MVC
{
    public class PlayerCtl : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed;

        //
        private Vector2 moveInput;
        private SpriteAnimCtl animator;

        private void Awake()
        {
            animator = GetComponent<SpriteAnimCtl>();
        }

        private void Update()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            // 保证斜向速度一致
            moveInput.Normalize();
            transform.position += (Vector3)moveInput * moveSpeed * Time.deltaTime;
            // 动画控制
            if (moveInput != Vector2.zero)
            {
                animator.SetMoving(true);

                if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                {
                    animator.SetDirection(moveInput.x > 0 ? Direction.Right : Direction.Left);
                }
                else
                {
                    animator.SetDirection(moveInput.y > 0 ? Direction.Up : Direction.Down);
                }
            }
            else
            {
                animator.SetMoving(false);
            }
        }
    }
}
