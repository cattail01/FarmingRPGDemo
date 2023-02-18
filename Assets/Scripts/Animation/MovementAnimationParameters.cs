
using System;
using Enums;

[Serializable]
public class MovementAnimationParameters
{
    // player animation parameters
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

    // shared animation Parameters
    public bool idleUp;
    public bool idleDown;
    public bool idleLeft;
    public bool idleRight;

}
