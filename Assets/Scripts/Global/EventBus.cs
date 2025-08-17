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
        private static readonly Dictionary<Type, Delegate> _table = new();

        // 订阅事件
        public static void Subscribe<T>(Action<T> handler)
        {
            if (handler == null)
            {
                // 宽松报错
                Debug.Log($"[EventBus] Subscribe<{typeof(T).Name}> 失败：handler 为 null");
                return;
            }
            var t = typeof(T);
            if (_table.TryGetValue(t, out var del))
            {
                // 合并委托
                _table[t] = Delegate.Combine(del, handler);
            }
            else
            {
                _table[t] = handler;
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
            if (_table.TryGetValue(t, out var del))
            {
                del = Delegate.Remove(del, handler);
                // 如果为空
                if (del == null)
                {
                    _table.Remove(t);
                }
                else
                {
                    _table[t] = del;
                }
            }
        }

        // 发布事件
        public static void Publish<T>(T evt)
        {
            if (_table.TryGetValue(typeof(T), out var del) && del is Action<T> cb)
            {
                cb.Invoke(evt);
            }
        }
    }
}
