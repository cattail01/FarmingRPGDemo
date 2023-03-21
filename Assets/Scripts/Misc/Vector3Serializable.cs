using System;


/// <summary>
/// 可序列化的vector3类
/// </summary>
/// <remarks>
/// serializable
/// for remain vector 3
/// </remarks>
[Serializable]
public class Vector3Serializable
{
    public float x, y, z;

    public Vector3Serializable(float x, float y, float z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public Vector3Serializable()
    {
    }
}
