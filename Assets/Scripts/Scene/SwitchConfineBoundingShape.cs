using Cinemachine;
using UnityEngine;

/// <summary>
/// <para>切换限制地图边界的形状</para>
/// <para>extend MonoBehavior：装在虚拟摄像机（cinemachine virtual camera）的游戏对象上，
/// 防止摄像机超出地图边界</para>
/// </summary>
[RequireComponent(typeof(CinemachineVirtualCamera))]
[RequireComponent(typeof(CinemachineConfiner))]
public class SwitchConfineBoundingShape : MonoBehaviour
{

    private void Awake()
    {

    }

    //private void Start()
    //{
    //    SwitchBoundingShape();
    //}

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundingShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundingShape;
    }



    /// <summary>
    /// 切换地图边缘碰撞器
    /// 这样 cinemachine 就能得到定义好的地图边界，防止摄像机超出边界
    /// </summary>
    private void SwitchBoundingShape()
    {
        // 获取游戏边界对象中的边界碰撞器
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();
        // 找到cinemachine对象的边界组件CinemachineConfiner
        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
        // 将地图边界碰撞器绑定在摄像机边界组件上
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
        // 清除缓存（因为边界切换）(英文：使路径缓存无效)
        cinemachineConfiner.InvalidatePathCache();
    }
}