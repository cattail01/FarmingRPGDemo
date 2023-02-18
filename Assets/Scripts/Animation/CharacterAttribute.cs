using Enums;

/// <summary>
/// 动画参数结构体
/// 标识了：
/// 动画属于人物哪一部分、
/// 动画是否有变体、
/// 动画变体类型
/// </summary>
[System.Serializable]
public struct CharacterAttribute
{
    // 表示该动画位于角色的哪个部位
    public CharacterPartAnimator characterPartAnimator;
    // 表示该部位动画是否有变体
    public PartVariantColour partVariantColour;
    // 表示该部位动画变体的类型
    public PartVariantType partVariantType;

    // 构造函数
    public CharacterAttribute(CharacterPartAnimator characterPartAnimator, PartVariantColour partVariantColour, PartVariantType partVariantType)
    {
        this.characterPartAnimator = characterPartAnimator;
        this.partVariantColour = partVariantColour;
        this.partVariantType = partVariantType;
    }
}
