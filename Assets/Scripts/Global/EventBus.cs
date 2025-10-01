using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    // 工具类，不需要继承单例
    public static class EventBus
    {
        // 保存事件类型(Type) -> 回调函数(Delegate) 的映射表
        // Delegate 支持多播，使用.Combine相当于构建函数列表，不需要用到List<>
        private static readonly Dictionary<Type, Delegate> _typedTable = new();

        // 只有订阅了相同 key 的回调会被唤醒
        private static readonly Dictionary<Type, Dictionary<object, Delegate>> _keyedTable = new();

        // 每个事件类型的最后一条消息（用于粘性重放）
        private static readonly Dictionary<Type, object> _last = new();

        // 无参包装器缓存
        private static readonly Dictionary<
            (Delegate original, Type eventType, object key),
            Delegate
        > _noArgWrappers = new Dictionary<(Delegate, Type, object), Delegate>();

        private static Delegate CombineDistinct(Delegate existing, Delegate added)
        {
            if (existing == null)
            {
                return added;
            }
            foreach (var d in existing.GetInvocationList())
            { 
                // 已存在则不重复合并
                if (d == added)
                {
                    return existing;
                }
            }
            return Delegate.Combine(existing, added);
        }

        // 订阅事件
        public static void Subscribe<T>(Action<T> handler, bool replayLast = true)
        {
            if (handler == null)
            {
                // 宽松报错
                Debug.Log($"[EventBus] Subscribe<{typeof(T).Name}> 失败：handler 为 null");
                return;
            }
            var t = typeof(T);
            if (_typedTable.TryGetValue(t, out var del))
            {
                // 去重式合并委托
                _typedTable[t] = CombineDistinct(del, handler);
            }
            else
            {
                _typedTable[t] = handler;
            }

            // 粘性重放（若有历史事件）
            if (replayLast && _last.TryGetValue(t, out var last))
            {
                try
                {
                    handler((T)last);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] 重放 {typeof(T).Name} 出错：{ex}");
                }
            }
        }

        // 移除订阅
        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null)
            {
                // 宽松报错
                Debug.Log($"[EventBus] Unsubscribe<{typeof(T).Name}> 失败：handler 为 null");
                return;
            }
            var t = typeof(T);
            if (_typedTable.TryGetValue(t, out var del))
            {
                del = Delegate.Remove(del, handler);
                // 如果为空
                if (del == null)
                {
                    _typedTable.Remove(t);
                }
                else
                {
                    _typedTable[t] = del;
                }
            }
        }

        // 发布事件
        public static void Publish<T>(T evt)
        {
            var t = typeof(T);
            _last[t] = evt!;
            if (_typedTable.TryGetValue(t, out var del) && del is Action<T> cb)
            {
                cb.Invoke(evt);
            }
        }

        // 订阅“子键频道”（类型 T + key）。将同一事件类型按 key 再细分成子通道
        // 好处：复用一个事件类型
        // 不用为每个 InputAction 分别定义 EPausePressed / EDialoguePressed 等事件
        // Action<T>表示handler必须有参
        public static void Subscribe<T, TKey>(TKey key, Action<T> handler)
        {
            if (handler == null)
            {
                // 宽松报错
                Debug.Log(
                    $"[EventBus] Subscribe<{typeof(T).Name},{typeof(TKey).Name}> 失败：handler 为 null"
                );
                return;
            }
            var t = typeof(T);
            if (!_keyedTable.TryGetValue(t, out var map))
            {
                // 若不存在则创建一个新的子键映射表
                map = new Dictionary<object, Delegate>();
                _keyedTable[t] = map;
            }
            var k = (object)key;
            // 若该 key 已经有订阅者
            if (map.TryGetValue(k, out var del))
            {
                // 合并成多播委托：后续 Publish 会依次调用所有订阅者
                map[k] = CombineDistinct(del, handler);
            }
            else
            {
                // 首个订阅者：直接存入委托
                map[k] = handler;
            }
        }

        // 订阅子键频道（无参Action）
        public static void Subscribe<T, TKey>(TKey key, Action handler)
        {
            if (handler == null)
            {
                // 宽松报错
                Debug.Log(
                    $"[EventBus] Subscribe<{typeof(T).Name},{typeof(TKey).Name}> 失败：handler 为 null"
                );
                return;
            }

            var t = typeof(T);
            var k = (object)key;

            // 复合键：用原始无参 handler + 事件类型 + 规范化子键 来唯一标识包装器
            var mapKey = ((Delegate)handler, t, k);

            if (!_noArgWrappers.TryGetValue(mapKey, out var wrapper))
            {
                // 创建包装器：将无参 handler 包装成 Action<T>，忽略 evt 参数
                Action<T> w = _ => handler();
                _noArgWrappers[mapKey] = w;

                // 复用已有的带参子键订阅实现
                Subscribe(key, w);
            }
            else
            {
                // 已存在包装器时重复订阅
                Subscribe(key, (Action<T>)wrapper);
            }
        }

        // 取消订阅子键频道
        public static void Unsubscribe<T, TKey>(TKey key, Action<T> handler)
        {
            if (handler == null)
            {
                // 宽松报错
                Debug.Log(
                    $"[EventBus] Unsubscribe<{typeof(T).Name},{typeof(TKey).Name}> 失败：handler 为 null"
                );
                return;
            }
            var t = typeof(T);
            if (_keyedTable.TryGetValue(t, out var map))
            {
                var k = (object)key;
                // 找到该 key 的多播委托
                if (map.TryGetValue(k, out var del))
                {
                    del = Delegate.Remove(del, handler);
                    // 若已无订阅者
                    if (del == null)
                    {
                        map.Remove(k);
                    }
                    else
                    {
                        map[k] = del;
                    }
                    // 如果此类型下已无任何 key 的订阅
                    if (map.Count == 0)
                    {
                        _keyedTable.Remove(t);
                    }
                }
            }
        }

        // 无参子键退订
        public static void Unsubscribe<T, TKey>(TKey key, Action handler)
        {
            if (handler == null)
            {
                Debug.Log(
                    $"[EventBus] Unsubscribe<{typeof(T).Name},{typeof(TKey).Name}> (no-arg) 失败：handler 为 null"
                );
                return;
            }

            var t = typeof(T);
            var k = (object)key;
            var mapKey = ((Delegate)handler, t, k);

            if (_noArgWrappers.TryGetValue(mapKey, out var wrapper))
            {
                // 用包装器去退订带参 keyed 表
                Unsubscribe<T, TKey>(key, (Action<T>)wrapper);
                // 清理缓存
                _noArgWrappers.Remove(mapKey);
            }
        }

        // 发布到“子键频道”（类型 T + key）：只唤醒订阅了同一 (T, key) 的回调（不会惊动其他键的订阅者）
        public static void Publish<T, TKey>(TKey key, T evt)
        {
            // 定位到事件类型 T 的子键映射表
            if (_keyedTable.TryGetValue(typeof(T), out var map))
            {
                var k = (object)key;
                if (map.TryGetValue(k, out var del) && del is Action<T> cb)
                {
                    cb.Invoke(evt);
                }
            }
        }
    }
}
