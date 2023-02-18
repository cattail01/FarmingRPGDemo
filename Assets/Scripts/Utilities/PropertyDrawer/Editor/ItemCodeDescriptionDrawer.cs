using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// 针对所有添加了ItemCodeDescription的特性的属性（字段）
/// 进行在editor上的重新绘制
/// 显示item code 所代表的item details
/// </summary>
[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    /// <summary>
    /// 设定字段在editor中的绘制高度
    /// </summary>
    /// <param name="property">参数</param>
    /// <param name="label"></param>
    /// <returns>修改后的label高度</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 检查传入的参数类型（就是item code）是否为序列化的int类型
        if (property.propertyType != SerializedPropertyType.Integer)
        {
            Debug.Log("[ItemCodeDescriptionDrawer.OnGUI]: property is not a serialized property integer type!");
            return;
        }
        // 开始修改gui上的参数
        EditorGUI.BeginProperty(position, label, property);
        // 开启进行在editor上改变数据的检查
        EditorGUI.BeginChangeCheck();
        // 绘制 item code 的部分（一个int的区域（field））（只需要一半的高度？）
        var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);
        // 绘制 item description（一个label 的 field）
        EditorGUI.LabelField(new Rect(position.x, position.y + (position.height / 2), position.width, (position.height / 2)), "Item Description", GetItemDescription(property.intValue));
        // 如果item code 的值改变了，就将值设置为新值
        if (EditorGUI.EndChangeCheck())
        {
            property.intValue = newValue;
        }
        // 结束修改
        EditorGUI.EndProperty();
    }

    /// <summary>
    /// 获取 item code 对应的 item details，并组成string返回
    /// </summary>
    /// <param name="itemCode">item code</param>
    /// <returns>item details string</returns>
    private string GetItemDescription(int itemCode)
    {
        SO_ItemList so_ItemList;
        // AssetDatabase能够查询assets文件夹中的文件
        // 这样就能通过assets中的相对路径加载想要的东西
        so_ItemList = AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjectAssets/Item/SO_ItemList.asset", typeof(SO_ItemList)) as SO_ItemList;
        if (so_ItemList == null)
        {
            throw new Exception();
        }
        // 根据 item code 查询数据
        List<ItemDetails> detailsList = so_ItemList.ItemsDetails;
        ItemDetails itemDetails = detailsList.Find(x => x.ItemCode == itemCode);
        if (itemDetails == null)
            return string.Empty;
        // 返回数据
        return itemDetails.ItemDescription;
    }
}
