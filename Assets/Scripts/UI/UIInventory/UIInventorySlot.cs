using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Enums;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler,
    IPointerExitHandler, IPointerClickHandler
{
    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;
    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;
    private Camera mainCamera;
    private Transform parentItem;
    [SerializeField] private UIInventoryBar inventoryBar;
    [SerializeField] private GameObject itemPrefab = null;


    [SerializeField] private int slotNumber;

    private GridCursor gridCursor;

    private void Awake()
    {
        inventoryBar = transform.parent.GetComponent<UIInventoryBar>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        gridCursor = FindObjectOfType<GridCursor>();
        //parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
        EventHandler.DropSelectedItemEvent += DropSelectedItemAtMousePosition;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        EventHandler.DropSelectedItemEvent -= DropSelectedItemAtMousePosition;
    }

    // 定义场景加载函数，注册事件
    public void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    // 因为在slot上拖拽而形成的物体（dragged item）
    private GameObject draggedItem;

    // 拖拽开始
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            // 对于player的控制取消，并重置运动状态
            PlayerSingletonMonoBehavior.Instance.DisablePlayerInputAndResetMovement();

            // 通过dragged item预制件，创建一个dragged item物体
            // 与教程不同的是，所有的inventoryBar的子物体是通过排列组件进行整理的，
            // 也就是说，不能将draggedItem创建为inventoryBar的子物体
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform.parent);

            // 获取拖拽的image
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
            //print(draggedItemImage.sprite.name);

            SetSelectedItem();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        draggedItem.transform.position = Input.mousePosition;
        //print(draggedItem.name);
        //print("draggedItem.transform.position.x" + draggedItem.transform.position.x +
        //      "\ndraggedItem.transform.position.y" + draggedItem.transform.position.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            Destroy(draggedItem);
            // eventData会发射射线，需要获取eventData.pointerCurrentRaycast指向的gameObject
            GameObject go = eventData.pointerCurrentRaycast.gameObject;
            // 如果成功地获取了物体，但是物体上有组件UIInventorySlot
            // 可以执行放置失败的方法，也可以执行交换两个item在inventory bar上的位置
            // swap
            if (go != null && go.GetComponent<UIInventorySlot>() != null)
            {
                int toSlotNumber = go.GetComponent<UIInventorySlot>().slotNumber;
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);
                DestroyInventoryTextBox();
                ClearSelectedItem();
            }
            else
            {
                if (itemDetails.CanBeDropped)
                {
                    DropSelectedItemAtMousePosition();
                }
            }
        }

        PlayerSingletonMonoBehavior.Instance.EnablePlayerInput();
    }

    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null && IsSelected)
        {
            // 将鼠标屏幕位置转化为世界位置
            // 因为摄像机z轴为-10，添加物体的目标位置为0，所以应当相对于摄像机坐标 + 10，就是-掉摄像机的z轴位置
            //Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            //    Input.mousePosition.y, -mainCamera.transform.position.z));

            //// 如果能够在该位置放置物体
            //Vector3Int gridPosition = GridPropertiesManager.Instance.Grid.WorldToCell(worldPosition);
            //GridPropertyDetails gridPropertyDetails =
            //    GridPropertiesManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);

            //if (gridPropertyDetails != null && gridPropertyDetails.CanDropItem)
            if(gridCursor.CursorPositionIsValid)
            {
                // 将鼠标屏幕位置转化为世界位置
                // 因为摄像机z轴为-10，添加物体的目标位置为0，所以应当相对于摄像机坐标 + 10，就是-掉摄像机的z轴位置
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, -mainCamera.transform.position.z));
                // 在世界中创建新item
                //GameObject go = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
                GameObject go = Instantiate(itemPrefab,
                    new Vector3(worldPosition.x, worldPosition.y - Settings.GridCellSize / 2f, worldPosition.z),
                    Quaternion.identity, parentItem);
                Item item = go.GetComponent<Item>();
                item.ItemCode = itemDetails.ItemCode;

                // 从inventory中拿出一个物体
                InventoryManager.Instance.RemoveOneItem(InventoryLocation.player, item.ItemCode);

                if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }
            }
        }
    }

    /// <summary>
    /// 在父物体中寻找的画布组件
    /// </summary>
    private Canvas parentCanvas;

    /// <summary>
    /// inventory text box prefab预制件
    /// </summary>
    [SerializeField] private GameObject inventoryTextBoxPrefab;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            // 通过预制件，创建InventoryTextBox对象
            inventoryBar.InventoryTextBoxGameObject =
                Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.InventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox =
                inventoryBar.InventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            // 获取itemTypeDescription
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.ItemType);

            // 给inventoryTextBox注入item description
            inventoryTextBox.SetTextBoxText(itemDetails.ItemName, itemTypeDescription, itemDetails.ItemDescription,
                itemDetails.ItemLongDescription, string.Empty, string.Empty);

            // 设置inventoryTextBox ui 物体的位置
            // 如果inventoryBar在屏幕下方
            if (inventoryBar.IsInventoryBarPositionButton)
            {
                inventoryBar.InventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.InventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x,
                    transform.position.y + 50f, transform.position.z);
            }
            // 如果inventoryBar在屏幕上方
            else
            {
                inventoryBar.InventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.InventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x,
                    transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    private void DestroyInventoryTextBox()
    {
        if (inventoryBar.InventoryTextBoxGameObject == null)
        {
            return;
        }

        Destroy(inventoryBar.InventoryTextBoxGameObject);
        inventoryBar.InventoryTextBoxGameObject = null;
    }

    [HideInInspector] public bool IsSelected;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (IsSelected)
            {
                ClearSelectedItem();
            }
            else
            {
                if (itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }

    private void ClearSelectedItem()
    {
        ClearCursors();

        inventoryBar.ClearHighlightOnInventorySlot();
        IsSelected = false;
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        // 清除角色变体动画，让角色的手放下来
        PlayerSingletonMonoBehavior.Instance.ClearCarriedItem();
    }

    private void SetSelectedItem()
    {
        inventoryBar.ClearHighlightOnInventorySlot();
        IsSelected = true;
        inventoryBar.SetHighlightOnInventorySlot();

        // cursor part


        gridCursor.ItemUseGridRadius = itemDetails.ItemUseGridRadius;

        if (itemDetails.ItemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }

        gridCursor.SelectedItemType = itemDetails.ItemType;


        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.ItemCode);
        CallAnimationOverride();
    }

    #region Animator Override Controller 相关部分

    private void CallAnimationOverride()
    {
        // 如果该物体能被携带，则使用携带变体动画
        if (itemDetails.CanBeCarried)
        {
            PlayerSingletonMonoBehavior.Instance.ShowCarriedItem(itemDetails.ItemCode);
        }
        // 如果不能携带，则清除以前所有的变体动画
        else
        {
            PlayerSingletonMonoBehavior.Instance.ClearCarriedItem();
        }
    }

    #endregion Animator Override Controller 相关部分

    #region 鼠标游标UI相关部分

    private void ClearCursors()
    {
        gridCursor.DisableCursor();

        gridCursor.SelectedItemType = ItemType.None;
    }

    #endregion
}
