using Enums;

/// <summary>
/// 网格属性类
/// </summary>
[System.Serializable]
public class GridProperty
{
    // 定义网格坐标
    public GridCoordinate GridCoordinate;

    // 定义网格bool参数枚举
    public GridBoolProperty GridBoolProperty;

    // 定义gridbool参数的值的bool值，表示确认与否
    public bool GridBoolValue = false;

    // 带参初始化构造函数
    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty, bool gridBoolValue)
    {
        GridCoordinate = gridCoordinate;
        GridBoolProperty = gridBoolProperty;
        GridBoolValue = gridBoolValue;
    }
}
