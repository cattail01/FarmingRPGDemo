using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    [SerializeField] private SO_NPCScheduleEventList soNpcScheduleEventList;

    private SortedSet<NPCScheduleEvent> npcScheduleEventSet;
    private NPCPath npcPath;

    private void GameTimeSystem_AdvanceMinute(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
        int gameHour, int gameMinute, int gameSecond)
    {
        int time = (gameHour * 100) + gameMinute;

        NPCScheduleEvent matchingNpcScheduleEvent = null;

        foreach (NPCScheduleEvent npcScheduleEvent in npcScheduleEventSet)
        {
            if (npcScheduleEvent.Time == time)
            {
                if (npcScheduleEvent.Day != 0 && npcScheduleEvent.Day != gameDay)
                {
                    continue;
                }

                if (npcScheduleEvent.Season != Season.None && npcScheduleEvent.Season != gameSeason)
                {
                    continue;
                }

                if (npcScheduleEvent.Weather != Weather.None &&
                    npcScheduleEvent.Weather != GameManager.Instance.CurrentWeather)
                {
                    continue;
                }

                matchingNpcScheduleEvent = npcScheduleEvent;
                break;
            }
            else if (npcScheduleEvent.Time > time)
            {
                break;
            }
        }
        if (matchingNpcScheduleEvent != null)
        {
            npcPath.BuildPath(matchingNpcScheduleEvent);
        }
    }

    private void Awake()
    {
        npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());

        foreach (NPCScheduleEvent npcScheduleEvent in soNpcScheduleEventList.NpcScheduleEventList)
        {
            npcScheduleEventSet.Add(npcScheduleEvent);
        }

        npcPath = GetComponent<NPCPath>();
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += GameTimeSystem_AdvanceMinute;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= GameTimeSystem_AdvanceMinute;
    }
}
