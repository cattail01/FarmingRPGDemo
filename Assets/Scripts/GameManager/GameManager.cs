using UnityEngine;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    protected override void Awake()
    {
        base.Awake();

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
}
