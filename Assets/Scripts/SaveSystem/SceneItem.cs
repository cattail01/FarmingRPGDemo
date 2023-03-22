using System;


/// <summary>
/// 场景中的物体的信息类
/// </summary>
/// <remarks>
/// <para>可序列化</para>
/// <para>为了存储场景中物体的信息</para>
/// </remarks>
[Serializable]
public class SceneItem
{
    // 定义物体编号
    public int ItemCode;

    // 物体名称
    public string ItemName;

    // 定义自定义的可序列化的vector3类
    // 用于记住物体在场景中的位置
    public Vector3Serializable Position;

    // 构造函数
    public SceneItem()
    {
        Position = new Vector3Serializable();
    }
}
