using UnityEngine;
using Enums;


/// <summary>
/// 描述动画类型的ScriptableObject类（数据类）
/// 让我们可以在editor中设置动画的类型，以及在代码中读取
/// </summary>
[CreateAssetMenu(fileName = "so_AnimationType", menuName = "ScriptableObject/Animation/AnimationType")]
public class SO_AnimationType : ScriptableObject
{
    // 在animator中的单个动画
    public AnimationClip animationClip;
    // 动画名称（当然是最基本的动画名称)
    public AnimationName animationName;
    // 动画属于主角的哪一个部分
    public CharacterPartAnimator characterPart;
    // 该动画是否由变体
    public PartVariantColour partVariantColour;
    // 该动画变体类型
    public PartVariantType partVariantType;
}
