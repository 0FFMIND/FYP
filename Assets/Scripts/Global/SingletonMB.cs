using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utils
{
    // 这里加上泛型约束，T必须是MonoBehaviour或者是子类
    // 继承MonoBehaviour 的单例实现会复杂一点，这是因为普通单例可以通过static Singleton()静态构造函数实现
    // CLR会在程序启动的时候自动调用静态构造函数
    // 而继承MonoBehaviour的类不能通过 new 创建，必须挂在 GameObject 上：new GameObject().AddComponent<T>()。这一步只能在引擎初始化后做
    public abstract class SingletonMB<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        private static T _instance;

        // 互斥锁
        private static readonly object _lock = new object();

        // 包装属性(外部访问器)
        public static T Instance
        {
            get
            {
                EnsureCreated();
                return _instance;
            }
        }

        private static void EnsureCreated()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    // 查找场景中是否存在
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }
                }
                DontDestroyOnLoad(_instance.gameObject);
            }
        }
    }

    // 顶层引导器：负责在“首场景加载前”就创建所有继承了 SingletonMB<> 的单例
    static class AutoSingletonBoot
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        // Unity 会在加载第一个场景之前调用此静态方法（仅一次）
        static void Boot()
        {
            foreach (var t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(SafeGetTypes))
            {
                if (t == null || t.IsAbstract || t.IsGenericType)
                    continue;
                if (!IsSubclassOfRawGeneric(t, typeof(SingletonMB<>)))
                    continue;

                try
                {
                    var closed = typeof(SingletonMB<>).MakeGenericType(t);
                    // 反射拿到该闭包类型上的 EnsureCreated
                    var m = closed.GetMethod(
                        "EnsureCreated",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                    );
                    // 调用 EnsureCreated()
                    m?.Invoke(null, null);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"AutoSingletonBoot 初始化 {t.Name} 失败: {e.Message}");
                }
            }
        }

        static bool IsSubclassOfRawGeneric(Type toCheck, Type raw)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (cur == raw)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        static Type[] SafeGetTypes(Assembly a)
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null).ToArray();
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }
    }
}
