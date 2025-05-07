using UnityEngine;

namespace Utils.SingletonPattern
{
    // 这里加上泛型约束，T必须是MonoBehaviour或者是子类
    public abstract class SingletonMB<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        // 互斥锁
        private static readonly object _lock = new object();
        // 包装属性(外部访问器)
        public static T Instance
        {
            get
            {
                // 线程安全
                lock (_lock)
                {
                    if(_instance == null)
                    {
                        // 查找场景中是否存在
                        _instance = FindObjectOfType<T>();
                        if(_instance == null)
                        {
                            var go = new GameObject(typeof(T).Name);
                            _instance = go.AddComponent<T>();
                        }
                    }
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
    }

}

