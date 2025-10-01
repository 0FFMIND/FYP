using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class PlayerInteractCtl : MonoBehaviour
    {
        private PlayerCtl root;
        private PlayerEmoteCtl emote;
        private PlayerModel playerModel;
        private IInteractable current; // 当前正在交互的对象引用
        private EmoteType _lastEmote = (EmoteType)(-1); // 上次已显示的类型
        private GameObject _lastGo;

        [SerializeField]
        private LayerMask interactMask;

        [SerializeField]
        private float rayLength = 0.8f; // 朝向射线长度

        [SerializeField]
        private float rayRadius = 0.5f; // 射线厚度

        // 结束后吞掉下一次 Interact 按键 (收到结束事件后，下一次按键被忽略)
        private bool _consumeNextInteractPress = false;

        // 吞键的自动失效时间点（Time.time），<0 表示无效
        private float _consumeDeadline = -1f;

        // 缓冲时长（单位：秒）
        private const float ConsumeBufferSeconds = 0.2f;

        private void Awake()
        {
            root = GetComponent<PlayerCtl>();
            emote = GetComponent<PlayerEmoteCtl>();
            // 获得引用
            playerModel = root.model;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EInputPressed, InputAction>(
                InputAction.DialogueClick,
                OnInteractPressed
            );
            EventBus.Subscribe<EInteractEnd>(OnInteractEnd);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EInputPressed, InputAction>(
                InputAction.DialogueClick,
                OnInteractPressed
            );
            EventBus.Unsubscribe<EInteractEnd>(OnInteractEnd);
        }

        private void OnInteractEnd(EInteractEnd _)
        {
            // 通知 Player 结束交互（释放移动/输入等）
            playerModel.EndInteract();
            // 在对话结束的时候标记，使下次交互按键直接吞掉，避免“结束瞬间又开始”
            _consumeNextInteractPress = true;
            // 设置 0.2s 自动失效
            _consumeDeadline = Time.time + ConsumeBufferSeconds;
        }

        private void Update()
        {
            // 自动清除：超过缓冲时间还没被消费，就取消吞键
            if (_consumeNextInteractPress && _consumeDeadline > 0f && Time.time >= _consumeDeadline)
            {
                _consumeNextInteractPress = false;
                _consumeDeadline = -1f;
            }
            // 每帧做一次“悬停气泡”刷新
            RefreshHoverEmote();
        }

        private void RefreshHoverEmote()
        {
            if (!emote)
                return;

            // 找最近可交互对象（你已有的方法）
            var target = FindInteractable();

            if (target == null)
            {
                if (_lastEmote != (EmoteType)(-1))
                {
                    emote.Stop(); // 离开可交互范围 → 收起气泡
                    _lastEmote = (EmoteType)(-1);
                    _lastGo = null;
                }
                return;
            }
            var go = (target as Component)?.gameObject;
            var _last = _lastGo;
            // 仅在目标变了时强制重算
            if (!ReferenceEquals(go, _lastGo))
            {
                _lastGo = go;
                _lastEmote = (EmoteType)(-1);
            }
            // 拿到 InteractCtl（含 isImportant/isTalked）
            var ic = (target as Component)?.GetComponent<InteractCtl>();
            // 规则：isImportant && !isTalked → Thinking，否则 Eyes
            var next =
                (ic != null && ic.IsImportant)
                    ? EmoteType.Thinking
                    : EmoteType.Eyes;
            if (ic.IsTalked)
            {
                next = EmoteType.Checked;
            }
            // 防抖：只有变化时才播放，避免反复重置动画
            if (next != _lastEmote)
            {
                _lastEmote = next;
                if (next == EmoteType.Checked /*&& ReferenceEquals(go, _last)*/)
                {
                    emote.Play(next, 1f, true);
                }else if (next == EmoteType.Checked)
                {
                    emote.Play(next, -1f, true);
                }
                else
                {
                    emote.Play(next, -1f);
                }
            }
        }

        private void OnInteractPressed()
        {
            // 若处于吞键状态：本次按键被忽略，并清除标记与计时
            if (_consumeNextInteractPress)
            {
                _consumeNextInteractPress = false;
                _consumeDeadline = -1f;
                return;
            }

            // 如果player处于不能交互的状态
            if (!playerModel.TryBeginInteract())
            {
                return;
            }

            // 在范围内查找最近的可交互对象
            current = FindInteractable();
            if (current == null || !current.BeginInteract(root))
            {
                // 找不到或被拒绝，则立刻结束交互并恢复
                current = null;
                playerModel.EndInteract();
            }
            else
            {
                emote.Stop();
            }
        }

        private IInteractable FindInteractable()
        {
            Vector2 origin = transform.position;
            // 从 PlayerModel 读取面向单位向量
            Vector2 dir = playerModel != null ? playerModel.FacingDir : Vector2.right;

            // 朝向粗射线
            var rayHits = Physics2D.CircleCastAll(origin, rayRadius, dir, rayLength, interactMask);
            IInteractable best = null;
            float bestDist = float.MaxValue;

            for (int i = 0; i < rayHits.Length; i++)
            {
                var h = rayHits[i];
                var it =
                    h.collider.GetComponentInParent<IInteractable>()
                    ?? h.collider.GetComponent<IInteractable>();
                if (it == null)
                    continue;

                // CircleCastHit2D.distance：命中点沿射线方向的距离，越小越近
                if (h.distance < bestDist)
                {
                    bestDist = h.distance;
                    best = it;
                }
            }
            return best;
        }

#if UNITY_EDITOR
        // 总是显示
        private void OnDrawGizmos()
        {
            // 射线起点与方向
            Vector2 origin = transform.position;
            Vector2 dir = Vector2.right;
            if (Application.isPlaying && playerModel != null)
                dir = playerModel.FacingDir;
            dir = dir.sqrMagnitude > 0f ? dir.normalized : Vector2.right;

            // 末端点
            Vector2 end = origin + dir * rayLength; // 终点 = 起点 + 方向*长度

            // 主线 + 两端圆帽（用两个圆表示厚度）
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, end);
            Gizmos.DrawWireSphere(origin, rayRadius);
            Gizmos.DrawWireSphere(end, rayRadius);

            // 命中点与法线
            if (Application.isPlaying) // 仅运行时绘制命中细节
            {
                var hits = Physics2D.CircleCastAll(origin, rayRadius, dir, rayLength, interactMask);
                for (int i = 0; i < hits.Length; i++)
                {
                    var h = hits[i];
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(h.point, 0.08f);
                    Gizmos.DrawLine(h.point, h.point + h.normal * 0.3f);
                }
            }
        }
#endif
    }
}
