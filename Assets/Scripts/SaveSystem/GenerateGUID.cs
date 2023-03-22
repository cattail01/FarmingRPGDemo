using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 产生全局统一标识符的类
/// </summary>
/// <remarks>
/// <para>在编译器里始终运行</para>
/// </remarks>
[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    // 定义GUID
    [SerializeField] private string guid = string.Empty;

    public string GUID
    {
        get => guid;
        set => guid = value;
    }

    private void Awake()
    {
        GetGUID();
    }

    // 定义获取机器标识符GUID的方法
    private void GetGUID()
    {
        // 游戏开始时不运行，仅在editor中运行
        if (Application.IsPlaying(gameObject))
            return;

        // 没有GUID则获取
        if (GUID == string.Empty)
        {
            GUID = System.Guid.NewGuid().ToString();
        }
    }
}
