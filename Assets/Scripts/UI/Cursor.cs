using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage;
    [SerializeField] private RectTransform cursorRectTransform;
    [SerializeField] private Sprite greenCursorSprite;
    [SerializeField] private Sprite transparentCursorSprite;
    [SerializeField] private GridCursor gridCursor;

    private bool _cursorIsEnabled = false;

    public bool CursorIsEnabled
    {
        get => _cursorIsEnabled;
        set => _cursorIsEnabled = value;
    }

    private bool _cursorPositionIsValid = false;

    public bool CursorPositionIsValid
    {
        get => _cursorPositionIsValid;
        set => _cursorPositionIsValid = value;
    }

    private ItemType _selectItemType;

    public ItemType SelectItemType
    {
        get => _selectItemType;
        set => _selectItemType = value;
    }

    private float _itemUseRadius = 0f;

    public float ItemUseRadius
    {
        get => _itemUseRadius;
        set => _itemUseRadius = value;
    }

    #region 脚本生命周期

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    #endregion 脚本生命周期

    private void DisplayCursor()
    {
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();
        SetCursorValidity(cursorWorldPosition, PlayerSingletonMonoBehavior.Instance.GetPlayerCenterPosition());
        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    public Vector3 GetWorldPositionForCursor()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    public Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
    }

    public void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid();

        if
        (
            cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) &&
            cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) &&
            cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) &&
            cursorPosition.y < (playerPosition.y - ItemUseRadius / 2)
            ||
            cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) &&
            cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f)
        )
        {
            SetCursorToInvalid();
            return;
        }

        if (Mathf.Abs(cursorPosition.x - playerPosition.x) > ItemUseRadius ||
            Mathf.Abs(cursorPosition.y - playerPosition.y) > ItemUseRadius)
        {
            SetCursorToInvalid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        switch (itemDetails.ItemType)
        {
            case ItemType.WateringTool:
            case ItemType.BreakingTool:
            case ItemType.ChoppingTool:
            case ItemType.HoeingTool:
            case ItemType.ReapingTool:
            case ItemType.CollectingTool:
                if (!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;
            case ItemType.None:
                break;
            case ItemType.Count:
                break;
            default:
                break;
        }

    }

    public void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
        gridCursor.DisableCursor();
    }

    public void SetCursorToInvalid()
    {
        cursorImage.sprite = transparentCursorSprite;
        CursorPositionIsValid = false;
        gridCursor.EnableCursor();
    }

    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        switch (itemDetails.ItemType)
        {
            case ItemType.ReapingTool:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);
                break;

            default:
                return false;
                break;
        }
    }

    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition,
        ItemDetails equippedItemDetails)
    {
        List<Item> itemlList = new List<Item>();
        if (HelperMethods.GetComponentsAtCursorLocation<Item>(out itemlList, cursorPosition))
        {
            if (itemlList.Count != 0)
            {
                foreach (Item item in itemlList)
                {
                    if (InventoryManager.Instance.GetItemDetailsByItemCode(item.ItemCode).ItemType ==
                        ItemType.ReapableScenery)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        CursorIsEnabled = false;
    }

//    public Vector3 GetWorldPositionForCursor()
//    {
//        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
//        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
//        return worldPosition;
//    }

    
}