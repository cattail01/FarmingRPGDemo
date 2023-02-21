using UnityEngine;
using Enums;

public class TimeManager : SingletonMonoBehavior<TimeManager>
{
    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;

    private string gameDayOfWeek = "Mon";

    // 游戏时钟是否暂停
    private bool gameClockPaused = false;

    private float gameTick = 0f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // 初始化游戏时间
        EventHandler.CallAdvanceGAmeMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        // 如果游戏时钟没有暂停
        if (!gameClockPaused)
        {
            // 游戏计时
            GameTick();
        }
    }

    /// <summary>
    /// 游戏计时器（技术重要）
    /// </summary>
    private void GameTick()
    {
        gameTick += Time.deltaTime;
        if (gameTick >= Settings.SecondsPerGameSecond)
        {
            gameTick -= Settings.SecondsPerGameSecond;
            UpdateGameSecond();
        }
    }

    // 更新游戏时间表(数字时钟基本算法)
    private void UpdateGameSecond()
    {
        // 游戏秒数增加
        ++gameSecond;
        if (gameSecond > 59)
        {
            gameSecond = 0;
            ++gameMinute;

            if (gameMinute > 59)
            {
                gameMinute = 0;
                ++gameHour;

                if (gameHour > 23)
                {
                    gameHour = 0;
                    ++gameDay;

                    // todo 如果希望更加真实，可以添加根据月份完成进位，包括月份判断和年份判断
                    if (gameDay > 30)
                    {
                        gameDay = 1;

                        int gs = (int)gameSeason;
                        ++gs;
                        gameSeason = (Season)gs;

                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;

                            ++gameYear;
                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek,
                                gameHour, gameMinute, gameSecond);
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour,
                            gameMinute, gameSecond);
                    }

                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour,
                        gameMinute, gameSecond);
                }
                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

            }
            EventHandler.CallAdvanceGAmeMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            Debug.Log(
                $"GameYear: {gameYear}\tGameSeason: {gameSeason.ToString()}\t" +
                $"GameDay: {gameDay}\tGameDayOfWeek: " +
                $"{gameDayOfWeek}\tGameHour: {gameHour}\tGameMinute: {gameMinute}");

        }
    }

    private string GetDayOfWeek()
    {
        int totalDay = (((int)gameSeason) * 30) + gameDay;
        int dayOfWeek = totalDay % 7;
        switch (dayOfWeek)
        {
            case 1:
                return "Mon";
            case 2:
                return "Tue";
            case 3:
                return "Wen";
            case 4:
                return "Thu";
            case 5:
                return "Fri";
            case 6:
                return "Sat";
            case 0:
                return "Sun";
            default:
                return "";
        }
    }
}
