
using Enums;
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

    // 定义 选中的物体类型
    private ItemType _selectedItemType;

    /// <summary>
    /// 选中的物体类型
    /// </summary>
    public ItemType SelectedItemType
    {
        get => _selectedItemType;
        set => _selectedItemType = value;
    }

    // 定义 游标启用标志
    private bool _cursorIsEnabled = false;

    public bool CursorIsEnabled
    {
        get => _cursorIsEnabled;
        set => _cursorIsEnabled = value;
    }

    #region 脚本生命周期

    private void Awake()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void Start()
    {

    }

    private void Update()
    {
        // 如果未开启cursor则跳过
        if (!CursorIsEnabled)
        {
            return;
        }

        // 显示 cursor
        DisplayCursor();
    }

    #endregion 脚本生命周期

    // 场景加载事件
    private void SceneLoaded()
    {

    }

    /// <summary>
    /// 显示 游标 的方法
    /// </summary>
    /// <returns></returns>
    private Vector3Int DisplayCursor()
    {
        // 如果网格为空，返回（0，0，0）
        if (grid == null)
        {
            return Vector3Int.zero;
        }

        // 获取鼠标的网格地址
        Vector3Int gridPosition = GetGridPositionForCursor();
        // 获取玩家的网格地址
        Vector3Int playerGridPosition = GetGridPositionForPlayer();

        // 设置那些游标是可以被激活的
        SetCursorValidity(gridPosition, playerGridPosition);

        // 获取 cursor 的 rect transform position
        cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

        return gridPosition;
    }

    /// <summary>
    /// 获取 网格位置 的方法
    /// </summary>
    /// <remarks>
    /// <para>该方法可用于为游标获取位置</para>
    /// <para>原理是鼠标转换到世界坐标，然后计算出该位置的cell坐标</para>
    /// </remarks>
    /// <returns>鼠标指向的cell坐标</returns>
    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            -mainCamera.transform.position.z));
        return grid.WorldToCell(worldPosition);
    }

    /// <summary>
    /// 获取 玩家网格位置 的方法
    /// </summary>
    /// <returns>玩家的网格位置</returns>
    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(PlayerSingletonMonoBehavior.Instance.transform.position);
    }

    public void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        // 设置游标为可用
        SetCursorToValid();

        // 如果超出使用范围，则设置游标为不可用
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius ||
            Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);
        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        GridPropertyDetails gridPropertyDetails =
            GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        switch (itemDetails.ItemType)
        {
            case ItemType.Seed:
                if (!IsCursorValidForSeed(gridPropertyDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;
            case ItemType.Commodity:
                if (!IsCursorValidForCommodity(gridPropertyDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;
            case ItemType.None:
                break;
            case ItemType.Count:
                break;
            default:
                break;
        }
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.CanDropItem;
    }

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.CanDropItem;
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;

        CursorIsEnabled = false;
    }
}
