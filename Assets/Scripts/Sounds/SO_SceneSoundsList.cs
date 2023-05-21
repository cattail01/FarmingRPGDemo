using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "soSceneSoundsList", menuName = "ScriptableObject/Sounds/SceneSoundsList")]
public class SO_SceneSoundsList : ScriptableObject
{
    [SerializeField] public List<SceneSoundsItem> SceneSoundsDetails;
}
