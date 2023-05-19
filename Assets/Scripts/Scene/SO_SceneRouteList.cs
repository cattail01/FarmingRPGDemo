using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "soSceneRouteList", 
    menuName = "ScriptableObject/Scene/SceneRouteList")]
public class SO_SceneRouteList : ScriptableObject
{
    public List<SceneRoute> SceneRouteList;
}
