using System;
using System.Collections.Generic;

/// <summary>
/// 场景保存类
/// </summary>
/// <remarks>
/// <para>用于集中放置场景中的物体状态或其他想要保存的信息</para>
/// </remarks>
[Serializable]
public class SceneSave
{
    // 定义名称到场景中物体信息列表的类
    // string name 是选择好的名称？？？？
    // 示例：`sceneSave.NameToSceneItemListDictionary.Add("sceneItemList", sceneItemList);`
    //public Dictionary<string, List<SceneItem>> NameToSceneItemListDictionary;

    // 定义存储SceneItem的列表
    public List<SceneItem> SceneItemList;

    // 定义string 到 地皮参数细节类 的字典
    public Dictionary<string, GridPropertyDetails> NameToGridPropertyDetailsDic;

    public Dictionary<string, bool> BoolDictionary;

    public Dictionary<string, string> StringDictionary;

    public Dictionary<string, Vector3Serializable> Vector3Dictionary;

    public List<InventoryItem>[] ListInvItemArray;

    public Dictionary<string, int[]> intArrayDictionary;
}
