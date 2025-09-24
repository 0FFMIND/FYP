using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class PlayerMoveCtl : MonoBehaviour
    {
        private PlayerCtl root;
        private PlayerModel model;
        private Rigidbody2D rb;
        private SpriteAnimCtl animator;

        private float moveSpeed;
        private float sprintMultiplier;

        private float speed;

        private void Awake()
        {
            root = GetComponent<PlayerCtl>();
            // 获得引用
            model = root.model;
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<SpriteAnimCtl>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EInputPressed, InputAction>(
                InputAction.PlayerSprint,
                OnPlayerSprint
            );
            EventBus.Subscribe<EInputUnPressed, InputAction>(
                InputAction.PlayerSprint,
                OnPlayerUnSprint
            );

            EventBus.Subscribe<ESettingsChanged>(SetSpeed);
        }
        private void OnPlayerSprint() => model.SetSprinting(true);

        private void OnPlayerUnSprint() => model.SetSprinting(false);

        private void OnDisable()
        {
            EventBus.Unsubscribe<EInputPressed, InputAction>(
                InputAction.PlayerSprint,
                OnPlayerSprint
            );
            EventBus.Unsubscribe<EInputUnPressed, InputAction>(
                InputAction.PlayerSprint,
                OnPlayerUnSprint
            );
            EventBus.Unsubscribe<ESettingsChanged>(SetSpeed);
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
            if (model.IsSprinting)
            {
                speed *= sprintMultiplier;
            }
            Vector2 input;
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            model.SetMoveInput(input);
            // 动画控制
            if (model.MoveInput != Vector2.zero)
            {
                animator.SetMoving(true);
                animator.SetDirection(model.Direction);
            }
            else
            {
                animator.SetMoving(false);
            }
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + model.MoveInput * speed * Time.fixedDeltaTime);
        }
    }
}
