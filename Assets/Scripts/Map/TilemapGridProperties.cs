using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

/// <summary>
/// 瓦片地图贴图参数类
/// </summary>
/// <remarks>
/// <para>继承自MonoBehaviour</para>
/// <para>用于自动保存所有的贴图参数信息</para>
/// </remarks>
[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SO_GridProperties gridProperties;

    // 定义瓦片参数enum，默认值为“可挖掘的”
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.Diggable;

    // 定义瓦片地图
    private Tilemap tilemap;

    private Grid grid;

    protected void OnEnable()
    {
        DoOnEnable();
    }

    private void OnDisable()
    {
        DoOnDisable();
    }

    // 在OnEnable中处理的内容
    private void DoOnEnable()
    {
        // 仅在editor中运行
        if (Application.IsPlaying(gameObject))
        {
            return;
        }

        // 获取tilemap组件
        tilemap = GetComponent<Tilemap>();

        // 如果 gridProperties 不为空，清空其中数据
        if (gridProperties != null)
        {
            gridProperties.GridPropertyList.Clear();
        }
    }

    // 在OnDisable中处理的内容
    private void DoOnDisable()
    {
        // 仅在editor中运行
        if (Application.IsPlaying(gameObject))
        {
            return;
        }

        // 更新场景参数
        UpdateGridProperties();

        // 如果gridProperties
        if (gridProperties != null)
        {
            // 将 grid properties 对象标记为脏对象
            EditorUtility.SetDirty(gridProperties);
        }
    }

    // 定义更新场景参数方法
    private void UpdateGridProperties()
    {
        // 压缩贴图边界
        tilemap.CompressBounds();

        //// 仅在editor中运行
        //if (Application.IsPlaying(gameObject))
        //{
        //    return;
        //}

        // 如果 gridProperties 为空，则什么都不做
        if (gridProperties == null)
        {
            return;
        }

        // 定义开始单元，并获取相关数据
        Vector3Int startCell = tilemap.cellBounds.min;

        // 定义结束单元，并获取相关数据
        Vector3Int endCell = tilemap.cellBounds.max;

        // 定义双层循环，对于每一个cell进行遍历
        for (int x = startCell.x; x < endCell.x; x++)
        {
            for (int y = startCell.y; y < endCell.y; y++)
            {
                // 获得 tile map 中的每一个 cell
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                if (tile != null)
                {
                    // 存储信息
                    gridProperties.GridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty,
                        true));
                }
            }
        }
    }
#endif
}
