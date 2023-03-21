using UnityEngine;

/// <summary>
/// 物品类
/// </summary>
public class Item : MonoBehaviour
{
    // 物品编号
    [ItemCodeDescription]
    [SerializeField]
    private int _itemCode;

    public int ItemCode
    {
        get => _itemCode;
        set => _itemCode = value;
    }

    /// <summary>
    /// 物品的精灵渲染组件
    /// </summary>
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    /// <summary>
    /// 使用填好的资料自动初始化item
    /// </summary>
    /// <param name="itemCodeParam">用于初始化item的物品编号</param>
    public void Init(int itemCodeParam)
    {
        if (itemCodeParam == 0)
        {
            return;
        }

        ItemCode = itemCodeParam;

        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetailsByItemCode(ItemCode);
        spriteRenderer.sprite = itemDetails.ItemSprite;
        if (itemDetails.ItemType == Enums.ItemType.ReapableScenery)
        {
            gameObject.AddComponent<ItemNudge>();
        }

    }
}
