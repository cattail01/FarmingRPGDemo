using Enums;
using UnityEngine;

/// <summary>
/// 物品的细节类
/// </summary>
[System.Serializable]
public class ItemDetails
{
    // 物品编号
    public int ItemCode;
    // 物品名称
    public string ItemName;
    // 物品种类
    public ItemType ItemType;
    // 物品描述
    public string ItemDescription;
    // 物品长描述
    public string ItemLongDescription;
    // 物品图片（精灵）
    public Sprite ItemSprite;
    // 物品使用的网格半径
    public short ItemUseGridRadius;
    // 物品使用的半径
    public float ItemUseRadius;
    // 是否是起始的物品
    public bool IsStartingItem;
    // 物品是否可以被拿起
    public bool CanBePickedUp;
    // 物品是否可以被放下
    public bool CanBeDropped;
    // 物品是否可以被吃掉
    public bool CanBeEaten;
    // 物品是否可以被携带（注意：可携带表示主人公可以将物体举在头顶）
    public bool CanBeCarried;
}

