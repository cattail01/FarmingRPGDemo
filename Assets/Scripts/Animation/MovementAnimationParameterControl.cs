using UnityEngine;

public class MovementAnimationParameterControl : MonoBehaviour
{
    private Animator animator;

    protected void Awake()
    {
        // 寻找组件
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            throw new ComponentCantBeFoundException(gameObject.name, "Animator",
                "MovementAnimationParameterControl", "Awake");
        }
    }

    protected void OnEnable()
    {
        // 向动画事件管理器中的事件添加函数
        EventHandler.MovementEvent += SetAnimationParameters;
    }

    protected void OnDisable()
    {
        // 从事件中删除设置动画参数的函数
        EventHandler.MovementEvent -= SetAnimationParameters;
    }

    // 设置动画转换的参数
    private void SetAnimationParameters
    (
        float inputX, float inputY,
        bool isWalking, bool isRunning, bool isIdle,
        bool isCarrying,
        Enums.ToolEffect toolEffect,
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
        bool isPackingRight, bool isPackingLeft, bool isPackingUp, bool isPackingDown,
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
        bool idleRight, bool idleLeft, bool idleUp, bool idleDown
    )
    {
        animator.SetFloat(Settings.xInput, inputX);
        animator.SetFloat(Settings.yInput, inputY);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);
        //animator.SetBool(Settings.);
        animator.SetInteger(Settings.toolEffect, (int)toolEffect);

        if (isUsingToolRight)
            animator.SetTrigger(Settings.isUsingToolRight);
        if (isUsingToolLeft)
            animator.SetTrigger(Settings.isUsingToolLeft);
        if (isUsingToolUp)
            animator.SetTrigger(Settings.isUsingToolUp);
        if (isUsingToolDown)
            animator.SetTrigger(Settings.isUsingToolDown);

        if (isLiftingToolRight)
            animator.SetTrigger(Settings.isLiftingToolRight);
        if (isLiftingToolLeft)
            animator.SetTrigger(Settings.isLiftingToolLeft);
        if (isLiftingToolUp)
            animator.SetTrigger(Settings.isLiftingToolUp);
        if (isLiftingToolDown)
            animator.SetTrigger(Settings.isLiftingToolDown);

        if (isPackingRight)
            animator.SetTrigger(Settings.isPickingRight);
        if (isPackingLeft)
            animator.SetTrigger(Settings.isPickingLeft);
        if (isPackingUp)
            animator.SetTrigger(Settings.isPickingUp);
        if (isPackingDown)
            animator.SetTrigger(Settings.isPickingDown);

        if (isSwingingToolRight)
            animator.SetTrigger(Settings.isSwingingToolRight);
        if (isSwingingToolLeft)
            animator.SetTrigger(Settings.isSwingingToolLeft);
        if (isSwingingToolUp)
            animator.SetTrigger(Settings.isSwingingToolUp);
        if (isSwingingToolDown)
            animator.SetTrigger(Settings.isSwingingToolDown);

        if (idleRight)
            animator.SetTrigger(Settings.idleRight);
        if (idleLeft)
            animator.SetTrigger(Settings.idleLeft);
        if (idleUp)
            animator.SetTrigger(Settings.idleUp);
        if (idleDown)
            animator.SetTrigger(Settings.idleDown);
    }

    /// <summary>
    /// 动画事件：播放脚步声
    /// </summary>
    private void AnimationEventPlayFootstepSound()
    {

    }
}
