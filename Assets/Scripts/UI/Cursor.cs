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

    }

}
