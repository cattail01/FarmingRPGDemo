using System;
using System.Collections.Generic;

/// <summary>
/// Scene Save Class
/// </summary>
/// <remarks>
/// <para>Place object or other something we want to store in scene</para>
/// </remarks>
[Serializable]
public class SceneSave
{
    //  define dictionary for String Name(?) to Scene Item List
    // string key is an identifier name we choose for this list
    public Dictionary<string, List<SceneItem>> ListSceneItemDictionary;

}
