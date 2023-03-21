using System;
using System.Collections.Generic;

[Serializable]
public class GameObjectSave
{
    // 定义 场景名称 to 场景保存的字典
    public Dictionary<string, SceneSave> sceneData;
}