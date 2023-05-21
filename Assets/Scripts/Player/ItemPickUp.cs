using System.Collections.Generic;
using Enums;
using UnityEngine;

/// <summary>
/// 对于地面上的物体识别并捡起
/// </summary>
public class ItemPickUp : MonoBehaviour
{
    /// <summary>
    /// 用于保存进入检测范围的物体列表
    /// </summary>
    private List<Item> items;
    private InventoryManager inventoryManager;
    private List<InventoryItem> inventoryList;

    private void Awake()
    {
        items = new List<Item>();
    }

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null)
        {
            throw new System.Exception();
        }
        inventoryList = inventoryManager.inventoryItemListArray[(int)Enums.InventoryLocation.player];
        if (inventoryList == null)
            throw new System.Exception();
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        // 获取碰撞器检查到的item
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            // 获取物品信息
            ItemDetails details = inventoryManager.GetItemDetailsByItemCode(item.ItemCode);
            if (details == null)
            {
                throw new System.Exception();
            }

            // 输出提示
            //print($"[{gameObject.name}: ItemPickUp.OnTriggerEnter2D]: {item.gameObject.name} is around here\n" +
            //$"long description is \"{details.ItemLongDescription}\"");

            // // 将物体（实际上是它的组件）的引用加入到周围物体中
            // // items.Add(item);

            // 如果该物体可以被捡起，且背包没满，执行捡起item的方法
             // // todo 将背包检查放到inventory manager的对应方法中
            if (inventoryManager.ItemDetailsDictionary[item.ItemCode].CanBePickedUp && inventoryList.Count < inventoryManager.inventoryListCapacityIntArray[(int)Enums.InventoryLocation.player])
            {
                inventoryManager.AddOneItem(Enums.InventoryLocation.player, item, item.gameObject);

                AudioManager.Instance.PlaySound(SoundName.EffectPickupSound);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 获取退出物体的组件
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            // 如果获取成功，输出提示信息
            //print($"[{gameObject.name}: ItemPickUp.OnTriggerExit2D]: {item.gameObject.name} is exit");

            // // 将物体的item组件从items中删除
            // // items.Remove(item);

        }
    }
}
