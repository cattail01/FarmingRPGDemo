using Enums;
using System;
using System.Collections.Generic;

[Serializable]
public class SceneRoute
{
    public SceneName FromSceneName;
    public SceneName ToSceneName;
    public List<ScenePath> ScenePathList;
}
