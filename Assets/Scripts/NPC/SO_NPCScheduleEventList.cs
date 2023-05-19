using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "soNPCScheduleEventList", 
    menuName = "ScriptableObject/NPC/NPCScheduleEventList")]
public class SO_NPCScheduleEventList : ScriptableObject
{
    [SerializeField] public List<NPCScheduleEvent> NpcScheduleEventList;
}
