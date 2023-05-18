// 定义所有的enum
namespace Enums
{
    #region 角色动画参数

    /// <summary>
    /// 工具产生的影响
    /// </summary>
    public enum ToolEffect
    {
        None,
        Watering
    }

    /// <summary>
    /// 角色移动的方向
    /// </summary>
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None,
    }

    #endregion

    #region 库存系统

    /// <summary>
    /// 物品类型
    /// </summary>
    public enum ItemType
    {
        Seed,   // 种子
        Commodity,  // 货物
        WateringTool,   // 浇水用的工具
        HoeingTool,     // 锄地工具
        ChoppingTool,   // 砍树工具
        BreakingTool,   // 破坏工具
        ReapingTool,    // 收割工具
        Furniture,      // 家具
        CollectingTool, // 收集工具
        ReapableScenery,    // 可以被收获的风景中的物体
        None,   // 啥也没有
        Count   // 种类总计
    }

    // 库存类型
    public enum InventoryLocation
    {
        player,
        chest,
        count
    }

    #endregion

    #region 动画变体

    // 人物所有的动画名称
    public enum AnimationName
    {
        IdleDown,
        IdleUp,
        IdleRight,
        IdleLeft,
        WalkUp,
        WalkDown,
        WalkRight,
        WalkLeft,
        RunUp,
        RunDown,
        RunRight,
        RunLeft,
        UseToolUp,
        UseToolDown,
        UseToolRight,
        UseToolLeft,
        SwingToolUp,
        SwingToolDown,
        SwingToolRight,
        SwingToolLeft,
        LiftToolUp,
        LiftToolDown,
        LiftToolRight,
        LiftToolLeft,
        HoldToolUp,
        HoldToolDown,
        HoldToolRight,
        HoldToolLeft,
        PickDown,
        PickUp,
        PickRight,
        PickLeft,
        Count
    }

    // 人物所有播放动画的位置
    public enum CharacterPartAnimator
    {
        body,
        arms,
        hair,
        tool,
        hat,
        count
    }

    // 部分动画变体
    public enum PartVariantColour
    {
        None,
        Count
    }

    // 变体动画的类型（携带物品时的动画变体、使用各种工具的动画变体等）
    public enum PartVariantType
    {
        None,
        Carry,
        Hoe,
        Axe,
        PickAxe,
        Scythe,
        WateringCan,
        Count
    }

    #endregion

    #region 游戏时间系统

    // 季节
    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter,
        None,
        Count
    }

    #endregion 游戏时间系统

    #region 游戏场景控制

    public enum SceneName
    {
        Scene1_Farm,
        Scene2_Field,
        Scene3_Cabin,
    }

    #endregion

    #region 瓦片地图参数信息

    /// <summary>
    /// 瓦片地图参数enum
    /// </summary>
    public enum GridBoolProperty
    {
        Diggable,   // 可挖掘的
        CanDropItem,    // 可以扔物体的
        CanPlaceFurniture, // 可以摆放家具的
        IsPath, // 是路径
        IsNpcObstacle,  // 是npc的障碍
    }

    #endregion 瓦片地图参数信息

    #region 收获部分

    public enum HarvestActionEffect
    {
        DeciduousLeavesFalling,
        PineConesFalling,
        ChoppingTreeTrunk,
        BreakingStone,
        Reaping,
        None,
    }

    #endregion

    #region 换装部分

    public enum Facing
    {
        None,
        Front,
        Back,
        Right
    }

    #endregion

    #region 游戏天气系统

    public enum Weather
    {
        None,
    }

    #endregion
}
