using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class Crop : MonoBehaviour
{
    /// <summary>
    /// 作物在网格中的位置
    /// </summary>
    [HideInInspector] public Vector2Int CropGridPosition;

    /// <summary>
    /// 收割动作计数器
    /// </summary>
    private int harvestActionCount = 0;

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        for (int i = 0; i < cropDetails.CropProducedItemCode.Length; ++i)
        {
            int cropsToProduce;

            if (cropDetails.CropProducedMinQuantity[i] == cropDetails.CropProducedMaxQuantity[i] ||
                cropDetails.CropProducedMaxQuantity[i] < cropDetails.CropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.CropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.CropProducedMinQuantity[i],
                    cropDetails.CropProducedMaxQuantity[i] + 1);
            }

            for (int j = 0; j < cropsToProduce; ++j)
            {
                Vector3 spawnPosition;
                if (cropDetails.SpawnCropProducedAtPlayerPosition)
                {
                    InventoryManager.Instance.AddOneItem(InventoryLocation.player, cropDetails.CropProducedItemCode[i]);
                    
                }
                else
                {
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f),
                        transform.position.y + Random.Range(-1f, 1f), 0f);
                    SceneItemManager.Instance.InstantiateSceneItem(cropDetails.CropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);

        Destroy(gameObject);
    }

    private void HarvestCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.SeedItemCode = -1;
        gridPropertyDetails.GrowthDays = -1;
        gridPropertyDetails.DaysSinceLastHarvest = -1;
        gridPropertyDetails.DaysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY,
            gridPropertyDetails);

        HarvestActions(cropDetails, gridPropertyDetails);
    }

    public void ProcessToolAction(ItemDetails equippedItemDetails)
    {
        GridPropertyDetails gridPropertyDetails =
            GridPropertiesManager.Instance.GetGridPropertyDetails(CropGridPosition.x, CropGridPosition.y);
        if (gridPropertyDetails == null)
        {
            return;
        }

        ItemDetails seedItemDetails =
            InventoryManager.Instance.GetItemDetailsByItemCode(gridPropertyDetails.SeedItemCode);
        if (seedItemDetails == null)
        {
            return;
        }

        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.ItemCode);
        if (cropDetails == null)
        {
            return;
        }

        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.ItemCode);
        if (requiredHarvestActions == -1)
        {
            return;
        }

        harvestActionCount += 1;

        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(cropDetails, gridPropertyDetails);
        }
    }
}
