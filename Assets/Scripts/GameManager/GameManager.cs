using Enums;
using UnityEngine;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    public Weather CurrentWeather;

    protected override void Awake()
    {
        base.Awake();

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        CurrentWeather = Weather.Dry;
    }
}
