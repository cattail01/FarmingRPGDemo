using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

// todo 效果是实现了，但是问题是canvas（或者摄像机）上的pixel per unity没有奏效，判定条件依然是屏幕坐标中一个单位为一个像素

public class UIInventoryBar : MonoBehaviour
{
    /// <summary>
    /// 获取GUI的RectTransform组件
    /// </summary>
    RectTransform rect;

    /// <summary>
    /// inventory bar在底部的时候为true，否则为false
    /// </summary>
    public bool IsInventoryBarPositionButton;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            throw new ComponentCantBeFoundException(gameObject.name, "RectTransform", "UIInventoryBar", "Awake");
        }
        IsInventoryBarPositionButton = true;
    }

    /// <summary>
    /// 获取主角player的instance
    /// </summary>
    PlayerSingletonMonoBehavior player;

    private void Start()
    {
        player = PlayerSingletonMonoBehavior.Instance;
        if (player == null)
        {
            throw new System.Exception();
        }
    }

    private void OnEnable()
    {
        // 订阅库存改变的事件，当库存发生更改时调用刷新库存图片的方法
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }


    private void OnDisable()
    {
        // 取消订阅库存改变的事件
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }


    private void Update()
    {
        SwitchInventorBarPosition();
    }

    private void SwitchInventorBarPosition()
    {
        // todo 优化：此处计算开销不小，优化时可以选择改成0.5秒或者更多的时间计算并执行一次
        // 获取玩家的视口上的位置
        Vector2 playerPositionToScreen = player.GetPlayerViewPositionOfMainCamera();
        print(playerPositionToScreen.y);
        // 如果玩家的y的位置不在下面，并且inventory bar在上面(不在底部)，将bar再移动回来
        if (!IsInventoryBarPositionButton && playerPositionToScreen.y > 0.3f)
        {
            InventoryBarToButton();
        }
        // 如果玩家的y的位置在下面，并且inventory bar在下面，将bar移动到上面去
        else if (IsInventoryBarPositionButton && playerPositionToScreen.y <= 0.3f)
        {
            InventoryBarToTop();
        }
    }

    private void InventoryBarToButton()
    {
        rect.pivot = new Vector2(0.5f, 0);
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.anchoredPosition = new Vector2(0f, 2.5f);
        IsInventoryBarPositionButton = true;
    }

    private void InventoryBarToTop()
    {
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -2.5f);
        IsInventoryBarPositionButton = false;
    }


    /// <summary>
    /// 存储下面所有的槽：slot
    /// </summary>
    [SerializeField]
    private UIInventorySlot[] inventorySlots;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private Sprite blank16x16sprite;

    /// <summary>
    /// 库存更新时自动调用该方法
    /// 功能：刷新inventory bar中的图片或数据
    /// </summary>
    /// <param name="inventoryLocation">inventory 的类型</param>
    /// <param name="inventoryItemList">inventory 的列表</param>
    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryItemList)
    {
        ClearInventorySlots();
        if (inventoryLocation != InventoryLocation.player)
            return;
        // 获取前bar.length个物品的sprite，并安装在bar slot上
        int length = Math.Min(inventorySlots.Length, inventoryItemList.Count);
        int itemCode;
        for (int i = 0; i < length; ++i)
        {
            InventoryItem inventoryItem = inventoryItemList[i];
            itemCode = inventoryItem.itemCode;
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetailsByItemCode(itemCode);
            if (itemDetails == null)
                continue;
            inventorySlots[i].inventorySlotImage.sprite = itemDetails.ItemSprite;
            inventorySlots[i].textMeshProUGUI.text = inventoryItem.itemQuantity.ToString();
            inventorySlots[i].itemDetails = itemDetails;
            inventorySlots[i].itemQuantity = inventoryItem.itemQuantity;
            SetHighlightOnInventorySlot(i);
        }
    }

    private void ClearInventorySlots()
    {
        int length = inventorySlots.Length;
        if (length <= 0)
        {
            return;
        }
        for (int i = 0; i < length; ++i)
        {
            inventorySlots[i].inventorySlotImage.sprite = blank16x16sprite;
            inventorySlots[i].textMeshProUGUI.text = "";
            inventorySlots[i].itemDetails = null;
            inventorySlots[i].itemQuantity = 0;
            SetHighlightOnInventorySlot(i);
        }
    }

    #region 扔出效果添加
    // 保存准备扔出的物品
    public GameObject inventoryBarDraggedItem;

    #endregion 扔出效果添加

    #region 存储inventoryTextBox对象
    public GameObject InventoryTextBoxGameObject;
    #endregion 存储inventoryTextBox对象

    public void ClearHighlightOnInventorySlot()
    {
        if (inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].IsSelected)
                {
                    inventorySlots[i].IsSelected = false;
                    inventorySlots[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    public void SetHighlightOnInventorySlot()
    {
        if (inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; ++i)
            {
                SetHighlightOnInventorySlot(i);
            }
        }
    }

    private void SetHighlightOnInventorySlot(int itemPosition)
    {
        if (inventorySlots.Length > 0 && inventorySlots[itemPosition].itemDetails != null)
        {
            if (inventorySlots[itemPosition].IsSelected)
            {
                inventorySlots[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlots[itemPosition].itemDetails.ItemCode);
            }

        }
    }
}
