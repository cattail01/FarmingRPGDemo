using UnityEngine;

// ? 不推荐使用单例类型
// ? 如果想要升级到多人游戏，则单例类可能会失效
// ? 整个游戏架构都会被大改动
// ? 不过到内个时候再说

/// <summary>
/// 抽象单例组件
/// </summary>
/// <typeparam name="T" type="MonoBehaviour">实现组件的类自己的类型</typeparam>
public abstract class SingletonMonoBehavior<T> : MonoBehaviour
    where T : MonoBehaviour
{
    public static T Instance;

    /// <summary>
    /// 给单例类赋值
    /// 如果其他单例类不存在，则生成单例类
    /// 如果其他单例类存在，则销毁正在创建单例类的对象
    /// </summary>
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

