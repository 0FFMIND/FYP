using System;

namespace Utils.SingletonPattern
{
    // abstractΪ�����࣬���ܱ�ʵ������ֻ�ܱ�����̳к�ʹ��
    // where���з���Լ����class������ֻ�����������ͣ�new()Լ���˱������public����internal���޲ι���
    // ����ͨ��new T()������T��ʵ��
    public abstract class Singleton<T> where T : class, new()
    {
        // static�������κ�ʵ��������ֻ����һ��
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());
        // ���Ⱪ¶
        public static T Instance = _instance.Value;
        // �������죬��ֹ�ⲿ��ʼ��
        protected Singleton() { }
    }
}

