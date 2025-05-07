using UnityEngine;

namespace Utils.SingletonPattern
{
    // ������Ϸ���Լ����T������MonoBehaviour����������
    public abstract class SingletonMB<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        // ������
        private static readonly object _lock = new object();
        // ��װ����(�ⲿ������)
        public static T Instance
        {
            get
            {
                // �̰߳�ȫ
                lock (_lock)
                {
                    if(_instance == null)
                    {
                        // ���ҳ������Ƿ����
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

