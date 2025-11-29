using Manager;
using System;
using System.Collections;
using UnityEngine;

namespace MVC
{
    public class PlayerScriptMoveCtl : MonoBehaviour
    {
        private float arriveEps = 0.1f;
        private PlayerCtl player;
        private Rigidbody2D rb;
        public PlayerAnimCtl anim;
        private bool _active; // 是否处于脚本驱动移动中（true 表示正在自动移动）
        private Vector2 _target;
        private float _speed;
        private bool _hasFaceOverride;
        private Direction _faceOverride;
        private Action _onArrive;

        private Coroutine _jumpCo;
        private SpriteRenderer[] _renderers; // 缓存所有渲染器（自身或子物体）

        public bool IsActive => _active;

        private void Awake()
        {
            player = GetComponent<PlayerCtl>();
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<PlayerAnimCtl>();
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
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

        // 强制更新贴图
        public void SetSprite(Sprite sprite)
        {
            anim.SetSprite(sprite);
        }

        public void SetLock(bool shouldLock)
        {
            anim.SetLock(shouldLock);
        }

        // 仅设置面朝方向（不移动），并立即生效
        public void SetFace(Direction dir)
        {
            _hasFaceOverride = true; // 打开“脚本强制朝向”
            _faceOverride = dir; // 记录强制朝向
            anim.SetMoving(false); // 明确告知动画机：当前为静止（避免行走帧）
            anim.SetDirection(dir); // 立即刷新朝向（本帧起效）
        }

        public Coroutine Jump(float totalTime, float height)
        {
            AudioMgr.Instance.PlaySFX("jump");
            if (_jumpCo != null) StopCoroutine(_jumpCo);
            _jumpCo = StartCoroutine(JumpCo(Mathf.Max(0.0001f, totalTime), height));
            return _jumpCo;
        }

        private IEnumerator JumpCo(float totalTime, float height)
        {
            // 保存并脱离物理
            bool sim = rb.simulated;
            Vector2 vel = rb.velocity;
            float angVel = rb.angularVelocity;
            float g = rb.gravityScale;
            rb.simulated = false;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;

            // sortingOrder +1
            int[] oldOrders = null;
            if (_renderers != null && _renderers.Length > 0)
            {
                oldOrders = new int[_renderers.Length];
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if (!_renderers[i]) continue;
                    oldOrders[i] = _renderers[i].sortingOrder;
                    _renderers[i].sortingOrder = oldOrders[i] + 1;
                }
            }

            // 做一个等速上/下的“V”形跳
            Vector3 startPos = transform.position;
            float half = totalTime * 0.5f;

            // 上升阶段: 0 -> height
            float t = 0f;
            while (t < half)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / half);
                transform.position = new Vector3(startPos.x, startPos.y + Mathf.Lerp(0f, height, u), startPos.z);
                yield return null;
            }

            // 下降阶段: height -> 0
            t = 0f;
            while (t < half)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / half);
                transform.position = new Vector3(startPos.x, startPos.y + Mathf.Lerp(height, 0f, u), startPos.z);
                yield return null;
            }

            // 归位
            transform.position = startPos;

            // 还原 sortingOrder
            if (oldOrders != null)
            {
                for (int i = 0; i < _renderers.Length; i++)
                    if (_renderers[i]) _renderers[i].sortingOrder = oldOrders[i];
            }

            // 恢复物理
            rb.simulated = sim;
            rb.velocity = vel;
            rb.angularVelocity = angVel;
            rb.gravityScale = g;

            _jumpCo = null;
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
