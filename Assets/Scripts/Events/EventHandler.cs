using System;
using System.Collections.Generic;
using System.Diagnostics;
using Enums;
using UnityEngine.Events;       // for UnityEvent

public delegate void MovementDelegate
(
    float inputX, float inputY,
    bool isWalking, bool isRunning, bool isIdle,
    bool isCarrying,
    Enums.ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown
);


public static class EventHandler
{
    #region 移动事件

    // 移动事件
    public static event MovementDelegate MovementEvent;

    // 定义调用移动事件的方法
    public static void CallMovementEvent
    (
        float inputX, float inputY,
        bool isWalking, bool isRunning, bool isIdle,
        bool isCarrying,
        Enums.ToolEffect toolEffect,
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
        bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
        bool idleRight, bool idleLeft, bool idleUp, bool idleDown
    )
    {
        if (MovementEvent != null)
            MovementEvent
            (
                inputX, inputY,
                isWalking, isRunning, isIdle,
                isCarrying,
                toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                idleRight, idleLeft, idleUp, idleDown
            );
    }


    #endregion

    #region 库存系统

    // 库存内容更改事件
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    // 库存内容更改事件调用
    public static void CallInventoryUpdatedEvent(InventoryLocation location, List<InventoryItem> itemList)
    {
        if (InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(location, itemList);
        }
    }

    #endregion

    #region 游戏时间系统事件

    /// <summary>
    /// 时间系统的Event，传入参数的含义见下方CallAdvanceGAmeMinuteEvent
    /// </summary>
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    /// <summary>
    /// 调用上方的时间系统event，参数见名知意
    /// </summary>
    public static void CallAdvanceGAmeMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
        int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
        int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
        int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
        int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek,
        int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    #endregion

    #region 游戏场景加载事件，属于游戏场景加载系统模块，供全局注册

    // 定义场景卸载淡出之前的事件
    public static event Action BeforeSceneUnloadFadeOutEvent;

    // 调用场景卸载淡出之前的事件
    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if (BeforeSceneUnloadFadeOutEvent == null)
        {
            return;
        }

        BeforeSceneUnloadFadeOutEvent();
    }

    // 定义场景卸载前的事件
    public static event Action BeforeSceneUnloadEvent;

    // 调用场景卸载前的事件
    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent == null)
        {
            return;
        }

        BeforeSceneUnloadEvent();
    }

    // 定义场景卸载后的事件
    public static event Action AfterSceneUnloadEvent;

    // 调用场景卸载后的事件
    public static void CallAfterSceneUnloadEvent()
    {
        if (AfterSceneUnloadEvent == null)
        {
            return;
        }
        AfterSceneUnloadEvent();
    }

    // 定义场景卸载前的事件
    public static event Action AfterSceneLoadEvent;

    // 调用场景卸载后的事件
    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    // 定义场景加载淡入事件
    public static event Action AfterSceneLoadFadeInEvent;

    // 调用场景加载淡入事件
    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }

    #endregion 游戏场景加载事件

    #region 扔出物体

    public static event Action DropSelectedItemEvent;

    public static void CallDropSelectedItemEvent()
    {
        if (DropSelectedItemEvent == null)
        {
            return;
        }

        DropSelectedItemEvent();
    }

    #endregion 扔出物体
}


