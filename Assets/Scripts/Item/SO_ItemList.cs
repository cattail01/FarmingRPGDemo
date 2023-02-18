using System.Collections.Generic;
using UnityEngine;

// fileName：文件名称
// menuName：在菜单中的名称
[CreateAssetMenu(fileName = "SO_ItemList", menuName = "ScriptableObject/Item/ItemList")]
public class SO_ItemList : ScriptableObject
{
    [SerializeField]
    public List<ItemDetails> ItemsDetails;
}
