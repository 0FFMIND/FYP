using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 工具类，不需要继承单例
public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> _table = new();
}
