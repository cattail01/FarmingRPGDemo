using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuInventoryManagementSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public Image InventoryManagementSlotImage;
    public TextMeshProUGUI TextMeshProUGUI;
    public GameObject GreyedOutImageGo;
    public GameObject DraggedItem;

    [SerializeField] private PauseMenuInventoryManagement inventoryManagement;
    [SerializeField] private GameObject inventoryTextBoxPrefab;
    [SerializeField] private int slotNumber = 0;

    [HideInInspector] public ItemDetails ItemDetails;
    [HideInInspector] public int itemQuantity;

    //private Vector3 startingPosition;
    private Canvas parentCanvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            DraggedItem = Instantiate(inventoryManagement.inventoryManagementDraggedItemPrefab,
                inventoryManagement.transform);
            Image draggedItemImage = DraggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = InventoryManagementSlotImage.sprite;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DraggedItem != null)
        {
            DraggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(DraggedItem);

        if (eventData.pointerCurrentRaycast.gameObject != null &&
            eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventoryManagementSlot>() != null)
        {
            int toSlotNumber = eventData.pointerCurrentRaycast.gameObject
                .GetComponent<PauseMenuInventoryManagementSlot>().slotNumber;
            InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);
            inventoryManagement.DestroyInventoryTextBoxGameObject();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            inventoryManagement.inventoryTextBoxGameObject =
                Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryManagement.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);
            UIInventoryTextBox inventoryTextBox =
                inventoryManagement.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(ItemDetails.ItemType);
            inventoryTextBox.SetTextBoxText(ItemDetails.ItemDescription, itemTypeDescription, string.Empty,
                ItemDetails.ItemLongDescription, string.Empty, string.Empty);

            if (slotNumber > 23)
            {
                inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot =
                    new Vector2(0.5f, 0f);
                inventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x,
                    transform.position.y + 50f, transform.position.z);
            }
            else
            {
                inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot =
                    new Vector2(0.5f, 1f);
                inventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x,
                    transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryManagement.DestroyInventoryTextBoxGameObject();
    }

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }
}
