using System;
using System.Collections.Generic;

[Serializable]
public class GameObjectSave
{
    // string key is SceneName
    public Dictionary<string, SceneSave> sceneData;
}