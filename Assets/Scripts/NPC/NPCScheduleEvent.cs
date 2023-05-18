using System;
using Enums;
using UnityEngine;

[Serializable]
public class NPCScheduleEvent
{
    public int Hour;
    public int Minute;
    public int Priority;
    public int Day;
    public Weather Weather;
    public Season Season;
    public SceneName ToSceneName;
    public GridCoordinate ToGridCoordinate;
    public Direction NpcFacingDirectionAtDestination = Direction.None;
    public AnimationClip AnimationAtDestination;

    public int Time
    {
        get => (Hour * 100) + Minute;
    }

    public NPCScheduleEvent()
    {

    }

    public NPCScheduleEvent(int hour, int minute, int priority, int day, Weather weather, Season season,
        SceneName toSceneName, GridCoordinate toGridCoordinate, AnimationClip animationAtDestination)
    {
        Hour = hour;
        Minute = minute;
        Priority = priority;
        Day = day;
        Weather = weather;
        Season = season;
        ToSceneName = toSceneName;
        ToGridCoordinate = toGridCoordinate;
        AnimationAtDestination = animationAtDestination;
    }

    public override string ToString()
    {
        //return base.ToString();
        return $"Time: {Time}, Priority: {Priority}, Day: {Day}, Weather: {Weather}, Season: {Season}";
    }
}
