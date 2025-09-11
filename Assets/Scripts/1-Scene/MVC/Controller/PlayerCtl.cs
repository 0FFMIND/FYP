using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class PlayerCtl : MonoBehaviour
    {
        private float moveSpeed;
        // 疾跑倍率
        private float sprintMultiplier;
  
        private Vector2 moveInput;
        private SpriteAnimCtl animator;
        private bool isSprint = false;
        Rigidbody2D rb;
        private float speed = 0f;

        private void Awake()
        {
            animator = GetComponent<SpriteAnimCtl>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EInputPressed, InputAction>(InputAction.PlayerSprint, OnPlayerSprint);
            EventBus.Subscribe<EInputUnPressed, InputAction>(InputAction.PlayerSprint, OnPlayerUnSprint);

            EventBus.Subscribe<ESettingsChanged>(SetSpeed);
        }

        private void OnPlayerSprint() => isSprint = true;

        private void OnPlayerUnSprint() => isSprint = false;


        private void OnDisable()
        {
            EventBus.Unsubscribe<EInputPressed, InputAction>(InputAction.PlayerSprint, OnPlayerSprint);
            EventBus.Unsubscribe<EInputUnPressed, InputAction>(InputAction.PlayerSprint, OnPlayerUnSprint);
        }

        private void SetSpeed(ESettingsChanged e)
        {
            moveSpeed = e.Settings.playerSpeed;
            sprintMultiplier = e.Settings.sprintMultiplier;
        }

        private void Update()
        {
            speed = moveSpeed;
            // 如果疾跑
            if (isSprint)
            {
                speed *= sprintMultiplier;
            }
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            // 保证斜向速度一致
            moveInput.Normalize();
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

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
        }
    }
}
