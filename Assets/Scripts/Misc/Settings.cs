
using UnityEngine;

/// <summary>
/// <para>
/// 生成并记录动画转换条件string的hash值
/// 以及其他参数
/// </para>
/// </summary>
public static class Settings
{

    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "Watering Can";
    public const string CollectingTool = "Basket";


    // 主角的初始仓库大小
    public static int PlayerInitialInventoryCapacity = 24;
    // 主角的最大仓库大小
    public static int PlayerMaxInventoryCapacity = 48;


    /// <summary>
    /// 淡入的时间
    /// </summary>
    public const float FadeInSeconds = 0.25f;

    /// <summary>
    /// 淡出的时间
    /// </summary>
    public const float FadeOutSeconds = 0.35f;

    /// <summary>
    /// 目标的透明值
    /// </summary>
    public const float TargetAlpha = 0.45f;



    public static float RunningSpeed = 5.333f;
    public static float WalkingSpeed = 2.666f;

    /// <summary>
    /// 使用工具动画暂停时间
    /// </summary>
    public static float UseToolAnimationPause = 0.25f;

    /// <summary>
    /// 用完工具动画的暂停时间
    /// </summary>
    public static float AfterUseToolAnimationPause = 0.2f;

    /// <summary>
    /// 浇水动画暂停时间
    /// </summary>
    public static float LiftToolAnimationPause = 0.4f;

    /// <summary>
    /// 浇水完成动画的暂停时间
    /// </summary>
    public static float AfterLiftAnimationPause = 0.4f;

    public static float PickAnimationPause = 1f;

    public static float AfterPickAnimationPause = 0.2f;

    // player animation parameters
    public static int xInput;
    public static int yInput;
    public static int isWalking;
    public static int isRunning;
    public static int toolEffect;
    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;
    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;
    public static int isSwingingToolRight;
    public static int isSwingingToolLeft;
    public static int isSwingingToolUp;
    public static int isSwingingToolDown;
    public static int isPickingRight;
    public static int isPickingLeft;
    public static int isPickingUp;
    public static int isPickingDown;

    // shared animation Parameters
    public static int idleUp;
    public static int idleDown;
    public static int idleLeft;
    public static int idleRight;

    public const float GridCellDiagonalSize = 1.41f;
    public static float PixelSize = 0.0625f;

    public static int WalkUp;
    public static int WalkDown;
    public static int WalkLeft;
    public static int WalkRight;
    public static int EventAnimation;

    public const int MaxGridWidth = 99999;
    public const int MaxGridHeight = 99999;

    /// <summary>
    /// 将animator 中的animation转换参数Parameters中的string转换为hashcode，
    /// 以便于更快的查找到该参数
    /// </summary>
    private static void AnimationTransitionParametersToHashCodeInt()
    {
        WalkUp = Animator.StringToHash("walkUp");
        WalkDown = Animator.StringToHash("walkDown");
        WalkLeft = Animator.StringToHash("walkLeft");
        WalkRight = Animator.StringToHash("walkRight");
        EventAnimation = Animator.StringToHash("eventAnimation");

        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        toolEffect = Animator.StringToHash("toolEffect");
        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");
        isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");
        isPickingRight = Animator.StringToHash("isPickingRight");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");

        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
        idleLeft = Animator.StringToHash("idleLeft");
        idleRight = Animator.StringToHash("idleRight");
    }


    public const float SecondsPerGameSecond = 0.012f;


    /// <summary>
    /// 地图单元大小
    /// </summary>
    public const float GridCellSize = 1f;

    /// <summary>
    /// 游标大小
    /// </summary>
    public static Vector2 CursorSize = Vector2.one;


    public static float PlayerCenterYOffset = 0.875f;



    public const int MaxCollidersToTestPerReapSwing = 15;
    public const int MaxTargetComponentsToDestroyPerReapSwing = 2;

    public const string PersistentScene = "PersistentScene";

    // constructor
    static Settings()
    {
        AnimationTransitionParametersToHashCodeInt();
    }
}
