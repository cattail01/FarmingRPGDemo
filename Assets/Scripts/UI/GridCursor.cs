
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 网格游标类
/// </summary>
/// <remarks>
/// <para>继承 mono behavior 类</para>
/// <para>是否可以放置物体可视化游标的主类</para>
/// </remarks>
public class GridCursor: MonoBehaviour
{
    // 定义 游标图片
    [SerializeField] private Image cursorImage = null;
    // 定义 游标位置
    [SerializeField] private RectTransform cursorRectTransform = null;
    // 定义 绿色游标精灵
    [SerializeField] private Sprite greenCursorSprite = null;
    // 定义 红色游标精灵
    [SerializeField] private Sprite redCursorSprite = null;
    // 定义 画布
    private Canvas canvas;
    // 定义 网格
    private Grid grid;
    // 定义 主摄像机
    private Camera mainCamera;

    // 定义 游标位置可用标志
    private bool _cursorPositionIsValid = false;

    /// <summary>
    /// 游标位置可用标志
    /// </summary>
    public bool CursorPositionIsValid
    {
        get => _cursorPositionIsValid;
        set => _cursorPositionIsValid = value;
    }

    // 定义 物体可用的网格半径
    private int _itemUseGridRadius = 0;

    /// <summary>
    /// 物体可用的网格半径
    /// </summary>
    public int ItemUseGridRadius
    {
        get => _itemUseGridRadius;
        set => _itemUseGridRadius = value;
    }
}
