using System;

namespace Utils.SingletonPattern
{
    // abstract为抽象类，不能被实例化，只能被子类继承后使用
    // where进行泛型约束，class限制了只能是引用类型，new()约束了必须具有public或者internal的无参构造
    // 才能通过new T()来创建T的实例
    public abstract class Singleton<T> where T : class, new()
    {
        // static不属于任何实例，有且只存在一份
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());
        // 对外暴露
        public static T Instance = _instance.Value;
        // 保护构造，防止外部初始化
        protected Singleton() { }
    }
}

