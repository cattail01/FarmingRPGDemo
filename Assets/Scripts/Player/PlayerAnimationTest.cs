
using Enums;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    //public MovementAnimationParameters animationParameters;
    // player animation parameters
    public float inputX;
    public float inputY;
    public bool isWalking;
    public bool isRunning;
    public bool isIdle;
    public bool isCarrying;
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

    protected void Update()
    {
        CallMovementEvent();
    }

    private void CallMovementEvent()
    {
        EventHandler.CallMovementEvent
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
}
