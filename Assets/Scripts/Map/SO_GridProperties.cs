using Enums;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图参数集合类
/// </summary>
/// <remarks>
/// <para>继承自ScriptableObject</para>
/// <para>表示grid property的集合</para>
/// </remarks>
[CreateAssetMenu(fileName = "SO_GridProperties", menuName = "ScriptableObject/GridProperties")]
public class SO_GridProperties: ScriptableObject
{
    // 定义表达的场景名称
    public SceneName SceneName;

    // 定义各种表达位置的参数

    public int GridWidth;
    public int GridHeight;
    public int OriginX;
    public int OriginY;

    // 定义参数集合列表，并使其可序列化
    [SerializeField] public List<GridProperty> GridPropertyList;
}
