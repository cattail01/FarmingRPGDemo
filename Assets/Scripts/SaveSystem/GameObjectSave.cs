using System;
using System.Collections.Generic;

/// <summary>
/// 游戏对象保存类
/// </summary>
/// <remarks>
/// <para></para>
/// </remarks>
[Serializable]
public class GameObjectSave
{
    // 定义 场景名称 to 场景保存的字典
    public Dictionary<string, SceneSave> sceneData_SceneNameToSceneSave;

    public GameObjectSave()
    {
        sceneData_SceneNameToSceneSave = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData_SceneNameToSceneSave = sceneData;
    }
}