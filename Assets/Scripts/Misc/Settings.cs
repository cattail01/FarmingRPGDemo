
using UnityEngine;

/// <summary>
/// <para>
/// 生成并记录动画转换条件string的hash值
/// 以及其他参数
/// </para>
/// </summary>
public static class Settings
{

    #region InventoryTextBox用到的一些字符串常量

    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "Watering Can";
    public const string CollectingTool = "Basket";

    #endregion

    #region 主角库存系统的相关参数

    // 主角的初始仓库大小
    public static int PlayerInitialInventoryCapacity = 24;
    // 主角的最大仓库大小
    public static int PlayerMaxInventoryCapacity = 48;

    #endregion

    #region  主角前遮挡物体透明的参数

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

    #endregion 主角前遮挡物体透明的参数

    #region 游戏角色的移动参数

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

    #endregion 游戏角色的移动参数

    #region 游戏角色动画状态改变参数的哈希值，用于找到游戏角色动画状态改变的参数

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

    /// <summary>
    /// 将animator 中的animation转换参数Parameters中的string转换为hashcode，
    /// 以便于更快的查找到该参数
    /// </summary>
    private static void AnimationTransitionParametersToHashCodeInt()
    {
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

    #endregion 游戏角色动画状态改变参数的哈希值，用于找到游戏角色动画状态改变的参数

    #region time system

    public const float SecondsPerGameSecond = 0.012f;

    #endregion time system

    #region 瓦片地图

    /// <summary>
    /// 地图单元大小
    /// </summary>
    public const float GridCellSize = 1f;

    /// <summary>
    /// 游标大小
    /// </summary>
    public static Vector2 CursorSize = Vector2.one;

    #endregion 瓦片地图

    #region 自由游标部分

    public static float PlayerCenterYOffset = 0.875f;

    #endregion

    #region 镰刀割草部分

    public const int MaxCollidersToTestPerReapSwing = 15;
    public const int MaxTargetComponentsToDestroyPerReapSwing = 2;

    #endregion

    // constructor
    static Settings()
    {
        AnimationTransitionParametersToHashCodeInt();
    }
}
