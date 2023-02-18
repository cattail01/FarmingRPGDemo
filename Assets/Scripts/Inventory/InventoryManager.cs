using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
/// 库存管理类：实现物品item中的itemCode与scriptable object中的资料的联系
/// </summary>
public class InventoryManager : SingletonMonoBehavior<InventoryManager>
{
    /// <summary>
    /// 强制在editor中显示所需要的SO_ItemList对象
    /// </summary>
    [SerializeField]
    private SO_ItemList itemList;

    /// <summary>
    /// 将物品的编号与物品细节信息关联起来的字典
    /// </summary>
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    public Dictionary<int, ItemDetails> ItemDetailsDictionary
    {
        get => itemDetailsDictionary;
        private set => itemDetailsDictionary = value;
    }

    // 表示仓库的列表的数组，意味着有多个列表组成的数组，每个列表存储InventoryItem元数据
    // 规定：下标数字与InventoryLocation枚举类型中的内容契合
    public List<InventoryItem>[] inventoryItemListArray;
    // 用于保存每种仓库的容积大小
    [HideInInspector]
    public int[] inventoryListCapacityIntArray;

    protected override void Awake()
    {
        if (itemList == null)
        {
            print($"[{gameObject.name}: InventoryManager.Start]: scriptable object <itemList: SO_ItemList> messed");
            throw new System.Exception();
        }
        // 执行基类awake
        // 保证基类的instance能够不为null
        base.Awake();
        // create item detail dictionary
        CreateItemDetailDictionary();
        // create item lists
        CreateItemLists();
        strBuilder = new StringBuilder();
        CreateItemTypeToDescriptionDic();
        CreateSelectedInventoryItemAwake();
    }


    /// <summary>
    /// 创建ItemDetailsDictionary
    /// 并向该字典中赋值
    /// </summary>
    private void CreateItemDetailDictionary()
    {
        ItemDetailsDictionary = new Dictionary<int, ItemDetails>();
        foreach (ItemDetails itemDetails in itemList.ItemsDetails)
        {
            ItemDetailsDictionary.Add(itemDetails.ItemCode, itemDetails);
        }
    }

    /// <summary>
    /// 工具方法
    /// 通过物品编号 item code，返回物品的详细数据
    /// <para>如果未找到该物品的数据，则返回null</para>
    /// </summary>
    /// <param name="itemCode">物品的编号</param>
    /// <returns>ItemDetails类型，表示物品的数据</returns>
    public ItemDetails GetItemDetailsByItemCode(int itemCode)
    {
        ItemDetails result;
        if (!ItemDetailsDictionary.TryGetValue(itemCode, out result))
        {
            print($"can't find value by key {itemCode}");
            return null;
        }
        return result;
    }

    /// <summary>
    /// 创建库存列表
    /// </summary>
    private void CreateItemLists()
    {
        // 创建inventoryItemListArray
        inventoryItemListArray = new List<InventoryItem>[(int)InventoryLocation.count];
        for (int i = 0; i < inventoryItemListArray.Length; ++i)
        {
            inventoryItemListArray[i] = new List<InventoryItem>();
        }
        // 创建inventoryListCapacityIntArray
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        // 初始化玩家仓库容积大小
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.PlayerInitialInventoryCapacity;
    }

    public void AddOneItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDisappear)
    {
        AddOneItemToList(item, inventoryLocation);
        // todo 如果性能不行的话，可以使用对象池优化
        Destroy(gameObjectToDisappear);
    }

    /// <summary>
    /// 向某个库存list中添加物品
    /// </summary>
    /// <param name="item">物品信息</param>
    /// <param name="inventoryLocation">想要放置的</param>
    public void AddOneItemToList(Item item, InventoryLocation inventoryLocation)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryItemListArray[(int)inventoryLocation];
        // 在容器列表中查找物体项目
        int itemPosition = -1;
        itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        // 如果找到了，则堆叠；否则找到空位置放置
        if (itemPosition != -1)
        {
            AddOneItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddOneItemAtPosition(inventoryList, itemCode);
        }

        // 调用inventory改变的事件
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryItemListArray[(int)inventoryLocation]);
    }


    /// <summary>
    /// 在库存中找到物体
    /// </summary>
    /// <param name="inventoryLocation">库存类型</param>
    /// <param name="itemCode">物体编号</param>
    /// <returns></returns>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryItemListArray[(int)inventoryLocation];
        return inventoryList.FindIndex(item => item.itemCode == itemCode);
    }

    private void AddOneItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int itemPosition)
    {
        if (itemPosition < 0 || itemPosition > inventoryList.Count)
            return;
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryList[itemPosition].itemQuantity;
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = quantity + 1;
        inventoryList[itemPosition] = inventoryItem;
        // Debug.ClearDeveloperConsole();
        Debug.Log($"[InventoryManager.AddItemAtPosition]: \nAdd a new item {itemDetailsDictionary[itemCode].ItemName} to ? inventory list ");
        DebugPrintInventoryList(inventoryList);
    }

    private void AddOneItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);
        // Debug.ClearDeveloperConsole();
        Debug.Log($"[InventoryManager.AddItemAtPosition]: \nAdd a new item {itemDetailsDictionary[itemCode].ItemName} to ? inventory list ");
        DebugPrintInventoryList(inventoryList);
    }

    // todo 前期debug使用的print代码段，后期记得优化掉（包括前面的new）
    StringBuilder strBuilder;
    private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    {
        strBuilder.Append("show inventory items in inventory list which is updated\n");
        int count = inventoryList.Count;
        strBuilder.Append($"list count is {count}\n");
        for (int i = 0; i < count; ++i)
        {
            strBuilder.Append($"item {i} is {itemDetailsDictionary[inventoryList[i].itemCode].ItemName}, ");
            strBuilder.Append($"number is {inventoryList[i].itemQuantity}\n");
        }
        Debug.Log(strBuilder.ToString());
        strBuilder.Clear();
    }

    // 从inventory中拿出一个物体
    public void RemoveOneItem(InventoryLocation inventoryLocation, int itemCode)
    {
        InventoryItem inventoryItem = inventoryItemListArray[(int)inventoryLocation].Find(inventoryItem => itemCode == inventoryItem.itemCode);
        // 如果是最后一个
        if (inventoryItem.itemQuantity <= 1)
        {
            inventoryItemListArray[(int)inventoryLocation].Remove(inventoryItem);
        }
        else
        {
            // 如果不是最后一个
            InventoryItem newInventoryItem = new InventoryItem();
            newInventoryItem.itemCode = inventoryItem.itemCode;
            newInventoryItem.itemQuantity = inventoryItem.itemQuantity - 1;
            int index = inventoryItemListArray[(int)inventoryLocation].FindIndex(inventoryItem => itemCode == inventoryItem.itemCode);
            inventoryItemListArray[(int)inventoryLocation][index] = newInventoryItem;
        }

        // 调用库存改变的事件
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryItemListArray[(int)inventoryLocation]);
    }

    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        int length = inventoryItemListArray[(int)inventoryLocation].Count;
        if (fromItem < length && toItem < length && fromItem != toItem && toItem >= 0 && fromItem >= 0)
        {
            InventoryItem fromInventoryItem = inventoryItemListArray[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryItemListArray[(int)inventoryLocation][toItem];

            inventoryItemListArray[(int)inventoryLocation][fromItem] = toInventoryItem;
            inventoryItemListArray[(int)inventoryLocation][toItem] = fromInventoryItem;

            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryItemListArray[(int)inventoryLocation]);
        }
    }

    public Dictionary<ItemType, string> ItemTypeToDescriptionDic;
    private void CreateItemTypeToDescriptionDic()
    {
        ItemTypeToDescriptionDic = new Dictionary<ItemType, string>();
        ItemTypeToDescriptionDic.Add(ItemType.BreakingTool, Settings.BreakingTool);
        ItemTypeToDescriptionDic.Add(ItemType.ChoppingTool, Settings.ChoppingTool);
        ItemTypeToDescriptionDic.Add(ItemType.HoeingTool, Settings.HoeingTool);
        ItemTypeToDescriptionDic.Add(ItemType.ReapingTool, Settings.ReapingTool);
        ItemTypeToDescriptionDic.Add(ItemType.WateringTool, Settings.WateringTool);
        ItemTypeToDescriptionDic.Add(ItemType.CollectingTool, Settings.CollectingTool);
    }

    /// <summary>
    /// 根据item type返回该type描述的字符串（在settings内定义的）
    /// </summary>
    /// <param name="itemType">物品的类型</param>
    /// <returns>物品的描述</returns>
    public string GetItemTypeDescription(ItemType itemType)
    {
        string result;
        if (!ItemTypeToDescriptionDic.TryGetValue(itemType, out result))
        {
            return itemType.ToString();
        }
        return result;
    }

    private int[] selectedInventoryItem;

    private void CreateSelectedInventoryItemAwake()
    {
        selectedInventoryItem = new int[(int)InventoryLocation.count];
        for (int i = 0; i < selectedInventoryItem.Length; ++i)
        {
            selectedInventoryItem[i] = -1;
        }
    }

    /// <summary>
    /// 为库存设置被选中的标记，从-1到该物体的 itemcode
    /// </summary>
    /// <param name="inventoryLocation">设置的是哪个库存</param>
    /// <param name="itemCode">物体编号</param>
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }

    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }
}
