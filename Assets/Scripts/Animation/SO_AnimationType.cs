using UnityEngine;
using Enums;


/// <summary>
/// 描述动画类型的类
/// </summary>
/// <remarks>
/// <para>继承自ScriptableObject</para>
/// </remarks>
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
