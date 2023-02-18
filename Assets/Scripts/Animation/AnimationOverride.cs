using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverride : MonoBehaviour
{
    // 保存当前人物的引用
    [SerializeField] private GameObject character;

    // 保存做好的动画类型 scriptable object 的实例对象的数组
    // 用于在编辑器中设置动画类型
    // （一大堆在editor中设置的数据）
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray;

    private void Start()
    {
        // 初始化动画类型的字典
        CreateAnimationTypeDictionaryByAnimation();
        CreateAnimationTypeDictionaryByCompositeAttributeKey();
    }

    /// <summary>
    /// 用于保存animationClip与SO_AnimationType对应关系的字典
    /// </summary>
    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;

    /// <summary>
    /// 根据animationClip，创建animationClip与SO_AnimationType对应的字典
    /// </summary>
    private void CreateAnimationTypeDictionaryByAnimation()
    {
        // 创建字典
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();
        // 初始化对应关系
        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }
    }

    // 通过SO_AnimationType中，所有类型enum的字符串组成的结构体，
    // 建立动画和其动画类型scriptable object 实例化对象的对应关系
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    /// <summary>
    /// 创建animationTypeDictionaryByCompositeAttributeKey字典
    /// </summary>
    private void CreateAnimationTypeDictionaryByCompositeAttributeKey()
    {
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();
        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            string key = item.characterPart.ToString() + item.partVariantColour.ToString() + item.partVariantType + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }
    }

    /// <summary>
    /// 对于角色动画使用定制化的 animator controller
    /// </summary>
    /// <param name="characterAttributeList">
    /// <para>
    /// 一个动画参数结构体组成的列表
    /// </para>
    /// </param>
    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributeList)
    {
        // 在人物上找到所有子物体的animator组件
        Animator[] animatorArray = character.GetComponentsInChildren<Animator>();

        // 遍历角色参数结构体组成的列表中的所有元素
        // （也就是对于参数进行逐一遍历）
        // * 该结构体包含了动画属于人物那一部分、动画是否有变体、动画变体类型等参数
        foreach (CharacterAttribute characterAttribute in characterAttributeList)
        {

            // 获取角色动画参数结构体中关于该动画位于角色那部分的信息
            string animatorSOAssetName = characterAttribute.characterPartAnimator.ToString();

            // 遍历所有找到的animator组件，找到该人物部位的animator
            Animator currentAnimator = null;
            foreach (Animator animator in animatorArray)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            if (currentAnimator == null)
            {
                throw new System.Exception();
            }

            // 创建AnimatorOverrideController
            // 装饰该部位animator的运行时动画控制器runtimeAnimatorController
            // 可以直接继承原来所有的animationClip
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);

            // 获取aoc的所有animatorClip
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            // 创建列表
            // 元素为 旧AnimationClip-新AnimationClip 的对应关系
            List<KeyValuePair<AnimationClip, AnimationClip>> animKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            // 对于该部位上animator下的所有AnimationClip进行遍历
            // 目的是对于每一个aoc上的animationClip，找到对应的新的animationClip
            // 具体操作步骤请看下面
            foreach (AnimationClip animationClip in animationsList)
            {
                // 根据旧的animationClip，在字典中找到对应的SO_AnimationType
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);
                if (foundAnimation)
                {
                    // 根据该动画的信息结构体与该动画的名称，在string字典中找到新的AnimationClip
                    string key = characterAttribute.characterPartAnimator.ToString() + characterAttribute.partVariantColour.ToString() + characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();
                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);
                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;
                        // 将旧的animationclip与新的animationclip组组成的对应，放入animKeyValuePairList中
                        animKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }
            // 使用animKeyValuePairList，将aoc的动画全部替换
            aoc.ApplyOverrides(animKeyValuePairList);
            // 最后将该animator的runtimeAnimatorController替换为aoc
            currentAnimator.runtimeAnimatorController = aoc;
        }
    }
}
