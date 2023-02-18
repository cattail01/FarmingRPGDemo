namespace Enums
{
    public enum ToolEffect
    {
        None,
        Watering
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None,
    }

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
        Scythe,
        WateringCan,
        Count
    }

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
}
