using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "soSoundList", 
    menuName = "ScriptableObject/Sounds/SoundList")]
public class SO_SoundList: ScriptableObject
{
    [SerializeField] public List<SoundItem> soundDetails;
}
