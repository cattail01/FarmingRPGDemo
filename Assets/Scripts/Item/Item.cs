using UnityEngine;

public class Item : MonoBehaviour
{
    [ItemCodeDescription]
    [SerializeField]
    private int _itemCode;

    public int ItemCode
    {
        get => _itemCode;
        set => _itemCode = value;
    }

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

    public void Init(int itemCodeParam)
    {
        if (itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetailsByItemCode(ItemCode);
            spriteRenderer.sprite = itemDetails.ItemSprite;
            if (itemDetails.ItemType == Enums.ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }
}
