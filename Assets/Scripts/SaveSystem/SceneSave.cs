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
    public Dictionary<string, List<SceneItem>> NameToSceneItemListDictionary;
}
