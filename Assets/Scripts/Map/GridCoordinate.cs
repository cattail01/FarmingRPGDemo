using UnityEngine;

/// <summary>
/// 网格坐标类
/// </summary>
[System.Serializable]
public class GridCoordinate
{
    // 保存x和y
    // 表示地图上的网格坐标
    public int x; public int y;

    // 定义构造函数，带参初始化
    public GridCoordinate(int x, int y)
    {
        this.x = x; this.y = y;
    }

    // 定义vector2操作符，将GridCoordinate显式转换为vector2
    public static explicit operator Vector2(GridCoordinate gridCoordinate)
    {
        return new Vector2((float)gridCoordinate.x, (float)gridCoordinate.y);
    }

    // 定义Vector2Int操作符，将GridCoordinate显式转换为Vector2Int
    public static explicit operator Vector2Int(GridCoordinate gridCoordinate)
    {
        return new Vector2Int(gridCoordinate.x, gridCoordinate.y);
    }

    // 定义Vector3操作符，将GridCoordinate显式转换为Vector3
    public static explicit operator Vector3(GridCoordinate gridCoordinate)
    {
        return new Vector3((float)gridCoordinate.x, (float)gridCoordinate.y, 0f);
    }

    // 定义Vector3Int操作符，将GridCoordinate显式转换为Vector3Int
    public static explicit operator Vector3Int(GridCoordinate gridCoordinate)
    {
        return new Vector3Int(gridCoordinate.x, gridCoordinate.y, 0);
    }
}
