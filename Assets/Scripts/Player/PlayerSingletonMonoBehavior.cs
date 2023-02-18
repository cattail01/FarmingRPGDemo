using Enums;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏主角单例组件
/// </summary>
public class PlayerSingletonMonoBehavior :
    SingletonMonoBehavior<PlayerSingletonMonoBehavior>
{
    private Camera mainCamera;

    #region Movement Parameters

    //private MovementAnimationParameters animationParameters;
    // player animation parameters
    private float xInput;
    private float yInput;
    private bool isWalking;
    private bool isRunning;
    private bool isIdle;
    private bool isCarrying;
    private bool isUsingToolRight;
    private bool isUsingToolLeft;
    private bool isUsingToolUp;
    private bool isUsingToolDown;
    private bool isLiftingToolRight;
    private bool isLiftingToolLeft;
    private bool isLiftingToolUp;
    private bool isLiftingToolDown;
    private bool isSwingingToolRight;
    private bool isSwingingToolLeft;
    private bool isSwingingToolUp;
    private bool isSwingingToolDown;
    private bool isPickingRight;
    private bool isPickingLeft;
    private bool isPickingUp;
    private bool isPickingDown;
    private bool idleUp;
    private bool idleDown;
    private bool idleLeft;
    private bool idleRight;

    private Enums.ToolEffect toolEffect = ToolEffect.None;

    private Enums.Direction playerDirection = Direction.None;

    private float movementSpeed;

    #endregion

    #region Components

    private new Rigidbody2D rigidbody2D;

    #endregion

    #region Play Input Controller

    private bool playerInputIsDisable = false;

    public bool PlayerInputIsDisable
    {
        get => playerInputIsDisable;
        set => playerInputIsDisable = value;
    }

    #endregion

    protected override void Awake()
    {
        // 创建单例的自己：instance实例化
        base.Awake();

        rigidbody2D = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        InitializeAnimatorOverrideControllerPart();
    }

    protected void Update()
    {
        PlayerMovementController();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovementController()
    {
        if (!playerInputIsDisable)
        {
            ResetAnimatorTriggers();
            PlayerInput();
            EventHandler.CallMovementEvent
            (
                xInput, yInput,
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

    private void ResetAnimatorTriggers()
    {
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        toolEffect = ToolEffect.None;
    }

    private void PlayerInput()
    {
        PlayerMovementInput();
        PlayerWalkInput();
    }

    private void PlayerMovementInput()
    {
        if (playerInputIsDisable)
            return;
        // 横向移动的键盘获取
        xInput = Input.GetAxisRaw("Horizontal");
        // 竖向移动的键盘获取
        yInput = Input.GetAxisRaw("Vertical");

        // 如果斜向跑动
        // 将每一个方向的移动 * 0.71（三角函数计算）
        if (xInput != 0 && yInput != 0)
        {
            xInput *= 0.71f;
            yInput *= 0.71f;
        }

        // 如果角色正在移动
        if (xInput != 0 || yInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;

            // 获得角色方向
            if (xInput < 0)
            {
                playerDirection = Direction.Left;
            }
            else if (xInput > 0)
            {
                playerDirection = Direction.Right;
            }
            else if (yInput < 0)
            {
                playerDirection = Direction.Down;
            }
            else
            {
                playerDirection = Direction.Up;
            }
        }
        else
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }
    }

    // 检查是否在行走
    private void PlayerWalkInput()
    {
        if (isIdle)
            return;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.WalkingSpeed;
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
        }

        movementSpeed = Settings.RunningSpeed;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();
        EventHandler.CallMovementEvent
        (
            xInput, yInput,
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

    private void ResetMovement()
    {
        xInput = 0f;
        yInput = 0f;
        isRunning = false;
        isIdle = false;
        movementSpeed = Settings.RunningSpeed;
    }

    public void DisablePlayerInput()
    {
        playerInputIsDisable = true;
    }

    public void EnablePlayerInput()
    {
        playerInputIsDisable = false;
    }

    private void PlayerMovement()
    {
        Vector2 moveVector2 = new Vector2(xInput * movementSpeed * Time.fixedDeltaTime,
            yInput * movementSpeed * Time.fixedDeltaTime);
        rigidbody2D.MovePosition(rigidbody2D.position + moveVector2);
    }

    public Vector2 GetPlayerViewPositionOfMainCamera()
    {
        // screen 表示屏幕坐标，以像素为单位
        // return mainCamera.WorldToScreenPoint(transform.position);
        // view表示视口坐标，左下为00， 右上为11
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    #region animator override controller 相关部分

    // AnimationOverride组件
    private AnimationOverride animationOverride;

    // 更改动画的列表
    private List<CharacterAttribute> characterAttributeList;

    // 在预制件中填充，用于显示举过头顶的物品
    [Tooltip("Should be populate in the prefab with the equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer;

    // 要更改动画的参数
    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;


    public void InitializeAnimatorOverrideControllerPart()
    {
        // get component
        animationOverride = GetComponentInChildren<AnimationOverride>();
        characterAttributeList = new List<CharacterAttribute>();

        // initialize CharacterAttributes 
        armsCharacterAttribute =
            new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.None, PartVariantType.None);
        toolCharacterAttribute =
            new CharacterAttribute(CharacterPartAnimator.tool,  PartVariantColour.None, PartVariantType.None);
    }

    public void ShowCarriedItem(int itemCode)
    {
        // 通过被举着物体的编号，获取该物体的细节
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetailsByItemCode(itemCode);
        if (itemDetails == null)
        {
            return;
        }

        // 将被举着的物品的图片替换为该物体的图片，并将其的alpha通道设置为1f可见
        equippedItemSpriteRenderer.sprite = itemDetails.ItemSprite;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

        // 应用人物的携带物体动画到 animation override
        armsCharacterAttribute.partVariantType = PartVariantType.Carry;
        characterAttributeList.Clear();
        characterAttributeList.Add(armsCharacterAttribute);
        animationOverride.ApplyCharacterCustomisationParameters(characterAttributeList);

        // 设置人物状态中的携带状态为true
        isCarrying = true;
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        armsCharacterAttribute.partVariantType = PartVariantType.None;
        characterAttributeList.Clear();
        characterAttributeList.Add(armsCharacterAttribute);
        animationOverride.ApplyCharacterCustomisationParameters(characterAttributeList);

        isCarrying = false;
    }

    #endregion animator override controller 相关部分
}
