using System;


/// <summary>
/// 可序列化的vector3类
/// </summary>
/// <remarks>
/// 序列化
/// 对vector3的重写
/// 表示位置等信息
/// </remarks>
[Serializable]
public class Vector3Serializable
{
    // 定义基本的位置
    public float x, y, z;

    public Vector3Serializable(float x, float y, float z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public Vector3Serializable()
    {
    }
}
