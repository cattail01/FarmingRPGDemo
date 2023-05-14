using Enums;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    public GameObject inventoryManagementDraggedItemPrefab;

    [SerializeField] private PauseMenuInventoryManagementSlot[] inventoryManagementSlots;
    [SerializeField] private Sprite transparent16x16;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private void InitialiseInventoryManagementSlots()
    {
        for (int i = 0; i < Settings.PlayerMaxInventoryCapacity; ++i)
        {
            inventoryManagementSlots[i].GreyedOutImageGo.SetActive(false);
            inventoryManagementSlots[i].ItemDetails = null;
            inventoryManagementSlots[i].itemQuantity = 0;
            inventoryManagementSlots[i].InventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagementSlots[i].TextMeshProUGUI.text = string.Empty;
        }

        for (int i = InventoryManager.Instance.inventoryListCapacityIntArray[(int)InventoryLocation.player];
             i < Settings.PlayerMaxInventoryCapacity;
             ++i)
        {
            inventoryManagementSlots[i].GreyedOutImageGo.SetActive(true);
        }
    }

    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, List<InventoryItem> playerInventoryList)
    {
        InitialiseInventoryManagementSlots();

        for (int i = 0; i < InventoryManager.Instance.inventoryItemListArray[(int)InventoryLocation.player].Count; ++i)
        {
            inventoryManagementSlots[i].ItemDetails =
                InventoryManager.Instance.GetItemDetailsByItemCode(playerInventoryList[i].itemCode);
            inventoryManagementSlots[i].itemQuantity = playerInventoryList[i].itemQuantity;

            if (inventoryManagementSlots[i].ItemDetails != null)
            {
                inventoryManagementSlots[i].InventoryManagementSlotImage.sprite =
                    inventoryManagementSlots[i].ItemDetails.ItemSprite;
                inventoryManagementSlots[i].TextMeshProUGUI.text = inventoryManagementSlots[i].itemQuantity.ToString();
            }
        }
    }

    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i < InventoryManager.Instance.inventoryItemListArray[(int)InventoryLocation.player].Count; ++i)
        {
            if (inventoryManagementSlots[i].DraggedItem != null)
            {
                Destroy(inventoryManagementSlots[i].DraggedItem);
            }
        }
    }

    public void DestroyInventoryTextBoxGameObject()
    {
        if (inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryTextBoxGameObject);
            inventoryTextBoxGameObject = null;
        }
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;

        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.player,
                InventoryManager.Instance.inventoryItemListArray[(int)InventoryLocation.player]);
        }
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;
        DestroyInventoryTextBoxGameObject();
    }
}
