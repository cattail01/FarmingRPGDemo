
using System;
using Enums;

/// <summary>
/// 移动的参数
/// </summary>
/// <remarks>
/// <para>note: 打算使用该类替代传递的超多参数</para>
/// </remarks>
[Serializable]
public class MovementAnimationParameters
{
    // 定义玩家动画参数
    public float xInput;
    public float yInput;
    public bool isWalking;
    public bool isRunning;
    public Enums.ToolEffect toolEffect;
    public bool isUsingToolRight;
    public bool isUsingToolLeft;
    public bool isUsingToolUp;
    public bool isUsingToolDown;
    public bool isLiftingToolRight;
    public bool isLiftingToolLeft;
    public bool isLiftingToolUp;
    public bool isLiftingToolDown;
    public bool isSwingingToolRight;
    public bool isSwingingToolLeft;
    public bool isSwingingToolUp;
    public bool isSwingingToolDown;
    public bool isPickingRight;
    public bool isPickingLeft;
    public bool isPickingUp;
    public bool isPickingDown;

    // 定义共有的动画参数
    public bool idleUp;
    public bool idleDown;
    public bool idleLeft;
    public bool idleRight;

}
