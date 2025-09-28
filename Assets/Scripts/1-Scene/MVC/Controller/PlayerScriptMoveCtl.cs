using System;
using UnityEngine;

namespace MVC
{
    public class PlayerScriptMoveCtl : MonoBehaviour
    {
        private float arriveEps = 0.01f;
        private PlayerCtl player;
        private Rigidbody2D rb;
        private SpriteAnimCtl anim;
        private bool _active; // 是否处于脚本驱动移动中（true 表示正在自动移动）
        private Vector2 _target;
        private float _speed;
        private bool _hasFaceOverride;
        private Direction _faceOverride;
        private Action _onArrive;

        public bool IsActive => _active;

        private void Awake()
        {
            player = GetComponent<PlayerCtl>();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<SpriteAnimCtl>();
        }
        public void StartMove(
            Vector3 worldPos,
            float? speed = -1,
            Direction? faceOverride = null,
            Action onArrive = null
        )
        {
            _target = worldPos;
            bool _hasSpeed = speed.HasValue;
            if (_hasSpeed)
            {
                _speed = speed.Value;
            }
            else
            {
                _speed = player.model.MoveSpeed;
            }
            _hasFaceOverride = faceOverride.HasValue;
            if (_hasFaceOverride)
            {
                _faceOverride = faceOverride.Value;
            }
            _onArrive = onArrive;
            _active = true;
        }

        // 仅设置面朝方向（不移动），并立即生效
        public void SetFace(Direction dir)
        {
            _hasFaceOverride = true; // 打开“脚本强制朝向”
            _faceOverride = dir; // 记录强制朝向
            anim.SetMoving(false); // 明确告知动画机：当前为静止（避免行走帧）
            anim.SetDirection(dir); // 立即刷新朝向（本帧起效）
        }

        private void Update()
        {
            if (!_active)
                return;

            Vector2 cur = rb.position;
            Vector2 to = _target - cur;
            if (to.sqrMagnitude <= arriveEps * arriveEps)
            {
                _active = false;
                anim.SetMoving(false);
                var cb = _onArrive;
                _onArrive = null;
                cb?.Invoke();
                return;
            }

            // 动画：行走 + 指定朝向（可与位移相反实现“后退”）
            anim.SetMoving(true);
            var face = _hasFaceOverride ? _faceOverride : VectorToDir(to);
            anim.SetDirection(face);
        }

        private void FixedUpdate()
        {
            if (!_active)
                return;
            Vector2 cur = rb.position;
            Vector2 dir = (_target - cur).normalized;
            rb.MovePosition(cur + dir * _speed * Time.fixedDeltaTime);
        }

        private Direction VectorToDir(Vector2 v)
        {
            if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            {
                return v.x >= 0 ? Direction.Right : Direction.Left;
            }
            else
                return v.y >= 0 ? Direction.Up : Direction.Down;
        }
    }
}
