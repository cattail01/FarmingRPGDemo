using System;
using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏主角
/// </summary>
public class PlayerSingletonMonoBehavior :
    SingletonMonoBehavior<PlayerSingletonMonoBehavior>, ISaveable
{
    #region 属性

    #region 运动动画参数

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

    #endregion 运动动画参数

    #endregion 属性

    /// <summary>
    /// 主摄像机
    /// </summary>
    private Camera mainCamera;


    private Enums.ToolEffect toolEffect = ToolEffect.None;

    private Enums.Direction playerDirection = Direction.None;

    private float movementSpeed;

    private new Rigidbody2D rigidbody2D;


    private bool playerInputIsDisable = false;

    public bool PlayerInputIsDisable
    {
        get => playerInputIsDisable;
        set => playerInputIsDisable = value;
    }

    private GridCursor gridCursor;

    // 主角禁用工具
    private bool playerToolUseDisable = false;

    // 使用工具暂停的时间
    private WaitForSeconds useToolAnimationPause;

    // 使用完工具暂停的时间
    private WaitForSeconds afterUseToolAnimationPause;

    // 使用浇水工具暂停的时间
    private WaitForSeconds liftToolAnimationPause;

    // 使用完浇水工具暂停的时间
    private WaitForSeconds afterLiftToolAnimationPause;

    //private WaitForSeconds afterSwingToolAnimationPause;

    private WaitForSeconds PickAnimationPause;

    private WaitForSeconds AfterPickAnimationPause;

    private string saveableUniqueId;

    public string SaveableUniqueId
    {
        get => saveableUniqueId;
        set => saveableUniqueId = value;
    }

    private GameObjectSave gameObjectSave;

    public GameObjectSave GameObjectSave
    {
        get => gameObjectSave;
        set => gameObjectSave = value;
    }

    protected override void Awake()
    {
        // 创建单例的自己：instance实例化
        base.Awake();

        rigidbody2D = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        InitializeAnimatorOverrideControllerPart();

        SaveableUniqueId = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        SaveableRegister();
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerInput;
    }

    private void OnDisable()
    {
        SaveableUnregister();
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerInput;
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();

        useToolAnimationPause = new WaitForSeconds(Settings.UseToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.AfterUseToolAnimationPause);

        liftToolAnimationPause = new WaitForSeconds(Settings.LiftToolAnimationPause);

        PickAnimationPause = new WaitForSeconds(Settings.PickAnimationPause);
        AfterPickAnimationPause = new WaitForSeconds(Settings.AfterPickAnimationPause);
    }

    protected void Update()
    {
        PlayerMovementController();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    public void SaveableStoreScene(string sceneName)
    {

    }

    public void SaveableRestoreScene(string sceneName)
    {

    }

    private void SetPlayerDirection(Direction playerDirection)
    {
        switch (playerDirection)
        {
            case Direction.Up:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.None, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                    false, true, false);
                break;
            case Direction.Down:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.None, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                    false, false, true);

                break;
            case Direction.Left:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.None, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                    true, false, false);
                break;
            case Direction.Right:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.None, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, true,
                    false, false, false);
                break;
            default:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.None, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                    false, false, true);
                break;
        }
    }

    public GameObjectSave SaveableSave()
    {
        GameObjectSave.sceneData_SceneNameToSceneSave.Remove(Settings.PersistentScene);
        SceneSave sceneSave = new SceneSave();
        sceneSave.Vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave.StringDictionary = new Dictionary<string, string>();
        Vector3Serializable vector3Serializable =
            new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
        sceneSave.Vector3Dictionary.Add("playerPosition", vector3Serializable);
        sceneSave.StringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);
        sceneSave.StringDictionary.Add("playerDirection", playerDirection.ToString());
        GameObjectSave.sceneData_SceneNameToSceneSave.Add(Settings.PersistentScene, sceneSave);
        return GameObjectSave;
    }

    public void SaveableLoad(GameSave gameSave)
    {
        if (gameSave.GameObjectData.TryGetValue(SaveableUniqueId, out GameObjectSave gameObjectSave))
        {
            if (gameObjectSave.sceneData_SceneNameToSceneSave.TryGetValue(Settings.PersistentScene,
                    out SceneSave sceneSave))
            {
                if (sceneSave.Vector3Dictionary != null &&
                    sceneSave.Vector3Dictionary.TryGetValue("playerPosition", out Vector3Serializable playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }

                if (sceneSave.StringDictionary != null)
                {
                    if (sceneSave.StringDictionary.TryGetValue("currentScene", out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }

                    if (sceneSave.StringDictionary.TryGetValue("playerDirection", out string playerDir))
                    {
                        bool playerDirFound = Enum.TryParse<Direction>(playerDir, true, out Direction direction);

                        if (playerDirFound)
                        {
                            playerDirection = direction;
                            SetPlayerDirection(playerDirection);
                        }
                    }
                }
            }
        }
    }

    public void SaveableRegister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Add(this);
    }

    public void SaveableUnregister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Remove(this);
    }

    private void PlayerMovementController()
    {
        //print(playerInputIsDisable);
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

        PlayerClickInput();

        // 用于测试游戏状态的输入
        PlayerTestInput();
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

    // AnimationOverride组件
    private AnimationOverride animationOverride;

    // 更改动画的列表
    private List<CharacterAttribute> characterAttributeList;

    // 在预制件中填充，用于显示举过头顶的物品
    [Tooltip("Should be populate in the prefab with the equipped item sprite renderer")] [SerializeField]
    private SpriteRenderer equippedItemSpriteRenderer;

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
            new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColour.None, PartVariantType.None);
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

    private void PlayerClickInput()
    {
        if (!playerToolUseDisable)
        {
            if (Input.GetMouseButton(0))
            {
                if (gridCursor.CursorIsEnabled || cursor.CursorIsEnabled)
                {
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();
                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

        // 获取游标位置的网格属性细节(GridCursor验证例程确保网格属性细节不为空)
        GridPropertyDetails gridPropertyDetails =
            GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);
        if (itemDetails == null)
        {
            return;
        }

        switch (itemDetails.ItemType)
        {
            case ItemType.Seed:
                if (Input.GetMouseButtonDown(0))
                {
                    ProcessPlayerClickInputSeed(gridPropertyDetails, itemDetails);
                }

                break;
            case ItemType.Commodity:
                if (Input.GetMouseButtonDown(0))
                {
                    ProcessPlayerClickInputCommodity(itemDetails);
                }

                break;
            case ItemType.HoeingTool:
            case ItemType.WateringTool:
            case ItemType.ReapingTool:
            case ItemType.CollectingTool:
            case ItemType.ChoppingTool:
            case ItemType.BreakingTool:
                ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                break;
            case ItemType.None:
                break;
            case ItemType.Count:
                break;
            default:
                break;
        }
    }


    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (itemDetails.CanBeDropped && gridCursor.CursorPositionIsValid && gridPropertyDetails.DaysSinceDug > -1 &&
            gridPropertyDetails.SeedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
        else if (itemDetails.CanBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.ItemCode) != null)
        {
            gridPropertyDetails.SeedItemCode = itemDetails.ItemCode;
            gridPropertyDetails.GrowthDays = 0;

            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

            EventHandler.CallRemoveSelectedItemFromInventoryEvent();
        }
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.CanBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }


    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails,
        Vector3Int playerDirection)
    {
        // 选择工具
        switch (itemDetails.ItemType)
        {
            case ItemType.HoeingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }

                break;
            case ItemType.WateringTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }

                break;
            case ItemType.ReapingTool:
                if (cursor.CursorPositionIsValid)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCenterPosition());
                    ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                }

                break;
            case ItemType.CollectingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }

                break;
            case ItemType.ChoppingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }

                break;
            case ItemType.BreakingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    BreakInputDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            default:
                break;
        }
    }

    private void BreakInputDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails,
        Vector3Int playerDirection)
    {
        StartCoroutine(BreakInputDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator BreakInputDirectionRoutine(GridPropertyDetails gridPropertyDetails,
        ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisable = true;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.PickAxe;
        characterAttributeList.Clear();
        characterAttributeList.Add(toolCharacterAttribute);
        animationOverride.ApplyCharacterCustomisationParameters(characterAttributeList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        PlayerInputIsDisable = false;
        playerToolUseDisable = false;
    }

    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails,
        Vector3Int playerDirection)
    {
        StartCoroutine(ChopInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator ChopInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails,
        ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisable = true;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.Axe;
        characterAttributeList.Clear();
        characterAttributeList.Add(toolCharacterAttribute);
        animationOverride.ApplyCharacterCustomisationParameters(characterAttributeList);

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        PlayerInputIsDisable = false;
        playerToolUseDisable = false;

        //yield return null;
    }

    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails,
        Vector3Int playerDirection)
    {
        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails,
        ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisable = true;
        playerToolUseDisable = true;

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);
        yield return PickAnimationPause;

        yield return AfterPickAnimationPause;

        PlayerInputIsDisable = false;
        playerToolUseDisable = false;
    }

    private void ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection,
        ItemDetails equippedItemDetails, GridPropertyDetails gridPropertyDetails)
    {
        switch (equippedItemDetails.ItemType)
        {
            case ItemType.CollectingTool:
                if (playerDirection == Vector3Int.right)
                {
                    isPickingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isPickingLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isPickingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isPickingDown = true;
                }

                break;

            case ItemType.ChoppingTool:
            case ItemType.BreakingTool:

                if (playerDirection == Vector3Int.right)
                {
                    isUsingToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isUsingToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isUsingToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isUsingToolDown = true;
                }

                break;

            case ItemType.None:
                break;
            default:
                break;
        }

        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);

        if (crop != null)
        {
            switch (equippedItemDetails.ItemType)
            {
                case ItemType.CollectingTool:
                case ItemType.BreakingTool:
                    crop.ProcessToolAction(equippedItemDetails, isPickingRight, isPickingLeft, isPickingDown,
                        isPickingUp);
                    break;

                case ItemType.ChoppingTool:
                    crop.ProcessToolAction(equippedItemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolDown,
                        isUsingToolUp);
                    break;
            }
        }
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        // trigger animation
        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        // trigger animation
        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisable = true;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.Hoe;
        characterAttributeList.Clear();
        characterAttributeList.Add(toolCharacterAttribute);
        animationOverride.ApplyCharacterCustomisationParameters(characterAttributeList);

        if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAnimationPause;

        // 设置挖掘状态为挖掘完成
        if (gridPropertyDetails.DaysSinceDug == -1)
        {
            gridPropertyDetails.DaysSinceDug = 0;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY,
            gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        PlayerInputIsDisable = false;
        playerToolUseDisable = false;
    }

    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisable = true;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.WateringCan;
        characterAttributeList.Clear();
        characterAttributeList.Add(toolCharacterAttribute);
        animationOverride.ApplyCharacterCustomisationParameters(characterAttributeList);

        toolEffect = ToolEffect.Watering;

        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        yield return liftToolAnimationPause;

        if (gridPropertyDetails.DaysSinceWatered == -1)
        {
            gridPropertyDetails.DaysSinceWatered = 0;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY,
            gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayWaterGround(gridPropertyDetails);

        yield return afterLiftToolAnimationPause;

        PlayerInputIsDisable = false;
        playerToolUseDisable = false;
    }

    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        //throw new NotImplementedException();

        PlayerInputIsDisable = true;
        playerToolUseDisable = true;

        toolCharacterAttribute.partVariantType = PartVariantType.Scythe;
        characterAttributeList.Clear();
        characterAttributeList.Add(toolCharacterAttribute);
        animationOverride.ApplyCharacterCustomisationParameters(characterAttributeList);

        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;
        PlayerInputIsDisable = false;
        playerToolUseDisable = false;
    }

    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            switch (equippedItemDetails.ItemType)
            {
                case ItemType.ReapingTool:
                    if (playerDirection == Vector3Int.right)
                    {
                        isSwingingToolRight = true;
                    }
                    else if (playerDirection == Vector3Int.left)
                    {
                        isSwingingToolLeft = true;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        isSwingingToolUp = true;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        isSwingingToolDown = true;
                    }

                    break;
            }

            Vector2 point =
                new Vector2(
                    GetPlayerCenterPosition().x + (playerDirection.x * (equippedItemDetails.ItemUseRadius / 2f)),
                    GetPlayerCenterPosition().y + playerDirection.y * (equippedItemDetails.ItemUseRadius / 2f));
            Vector2 size = new Vector2(equippedItemDetails.ItemUseRadius, equippedItemDetails.ItemUseRadius);
            Item[] itemArray =
                HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.MaxCollidersToTestPerReapSwing, point,
                    size, 0f);
            int reapableItemCount = 0;

            for (int i = itemArray.Length - 1; i >= 0; --i)
            {
                if (itemArray[i] != null)
                {
                    if (InventoryManager.Instance.GetItemDetailsByItemCode(itemArray[i].ItemCode).ItemType ==
                        ItemType.ReapableScenery)
                    {
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x,
                            itemArray[i].transform.position.y + Settings.GridCellSize / 2f,
                            itemArray[i].transform.position.z);

                        // trigger reaping effect
                        EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.Reaping);

                        Destroy(itemArray[i].gameObject);
                        reapableItemCount++;
                        if (reapableItemCount >= Settings.MaxTargetComponentsToDestroyPerReapSwing)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    // 获取主角点击的方向
    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        //throw new NotImplementedException();
        if (
            cursorPosition.x > playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
        )
        {
            return Vector3Int.right;
        }
        else if
        (
            cursorPosition.x < playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
        )
        {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private Cursor cursor;

    public Vector3 GetPlayerCenterPosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.PlayerCenterYOffset,
            transform.position.z);
    }

    // todo remove
    private void PlayerTestInput()
    {
        // 按t键加快分钟
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }

        // 按g键加快年份
        if (Input.GetKey(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }

        //// 按l测试场景切换
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    GameObject tree = PoolManager.Instance.ReuseObject(CanyonOakTreePrefab,
        //        mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
        //            -mainCamera.transform.position.z)), Quaternion.identity);
        //    tree.SetActive(true);
        //}
    }

    public GameObject CanyonOakTreePrefab;
}
