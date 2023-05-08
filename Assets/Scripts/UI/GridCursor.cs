using System.Collections.Generic;
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
public class GridCursor : MonoBehaviour
{
    #region 属性

    #region Serialize filed

    // 游标图片
    [SerializeField] private Image cursorImage = null;

    // 游标变换
    [SerializeField] private RectTransform cursorRectTransform = null;

    // 绿色游标精灵
    [SerializeField] private Sprite greenCursorSprite = null;

    // 红色游标精灵
    [SerializeField] private Sprite redCursorSprite = null;

    // 作物信息列表
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;

    #endregion Serialize filed

    #region Private

    // 画布
    private Canvas canvas;

    // 网格
    private Grid grid;

    // 主摄像机
    private Camera mainCamera;

    // 游标位置可用标志
    private bool _cursorPositionIsValid = false;

    // 物体可用的网格半径
    private int _itemUseGridRadius = 0;

    // 选中的物体类型
    private ItemType _selectedItemType;

    // 游标启用标志
    private bool _cursorIsEnabled = false;

    #endregion Private

    #region Public Visitor

    /// <summary>
    /// 游标位置可用标志
    /// </summary>
    public bool CursorPositionIsValid
    {
        get => _cursorPositionIsValid;
        set => _cursorPositionIsValid = value;
    }

    /// <summary>
    /// 物体可用的网格半径
    /// </summary>
    public int ItemUseGridRadius
    {
        get => _itemUseGridRadius;
        set => _itemUseGridRadius = value;
    }

    /// <summary>
    /// 选中的物体类型
    /// </summary>
    public ItemType SelectedItemType
    {
        get => _selectedItemType;
        set => _selectedItemType = value;
    }

    /// <summary>
    /// 游标启用标志
    /// </summary>
    public bool CursorIsEnabled
    {
        get => _cursorIsEnabled;
        set => _cursorIsEnabled = value;
    }

    #endregion Public Visitor

    #endregion 属性


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
        grid = GameObject.FindObjectOfType<Grid>();
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
            case ItemType.WateringTool:
            case ItemType.BreakingTool:
            case ItemType.ChoppingTool:
            case ItemType.HoeingTool:
            case ItemType.ReapingTool:
            case ItemType.CollectingTool:
                if (!IsCursorValidForTool(gridPropertyDetails, itemDetails))
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

    /// <summary>
    /// （译）将光标设置为目标gridPropertyDetails的工具的有效或无效。i有效时返回true，无效时返回false
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <param name="itemDetails"></param>
    /// <returns></returns>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        // 选择工具
        switch (itemDetails.ItemType)
        {
            case ItemType.HoeingTool:
                // 能够挖掘的条件
                if (gridPropertyDetails.IsDiggable == true && gridPropertyDetails.DaysSinceDug == -1)
                {
                    #region 需要在现场获取任何物品，以便我们检查它们是否可回收

                    // 获取游标的世界位置
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f,
                        GetWorldPositionForCursor().y + 0.5f, 0f);

                    // 定义 在游标位置，所有物体的列表
                    List<Item> itemList = new List<Item>();

                    // 获取游标位置的所有物体
                    HelperMethods.TryGetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition,
                        Settings.CursorSize, 0f);

                    #endregion

                    // 循环遍历所找到的items，看看是否有可回收的类型-我们不会让玩家挖掘哪里有可回收的场景
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetailsByItemCode(item.ItemCode).ItemType ==
                            ItemType.ReapableScenery)
                        {
                            foundReapable = true;
                            break;
                        }
                    }

                    if (foundReapable)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }

                break;
            case ItemType.WateringTool:
                if (gridPropertyDetails.DaysSinceDug > -1 && gridPropertyDetails.DaysSinceWatered == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case ItemType.ChoppingTool:
            case ItemType.CollectingTool:
            case ItemType.BreakingTool:
                if (gridPropertyDetails.SeedItemCode != -1)
                {
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.SeedItemCode);
                    if (cropDetails != null)
                    {
                        if (gridPropertyDetails.GrowthDays >= cropDetails.GrowthDays[cropDetails.GrowthDays.Length - 1])
                        {
                            if (cropDetails.CanUseToolToHarvestCrop(itemDetails.ItemCode))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else
                        {
                            return false;
                        }
                    }

                }

                break;
            default:
                return false;
        }

        return false;
    }

    /// <summary>
    /// 获取游标的世界位置
    /// </summary>
    /// <returns>游标的世界位置</returns>
    private Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
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
