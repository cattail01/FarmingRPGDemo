using Enums;
using System;
using UnityEngine;


[Serializable]
public class CropDetails
{
    [ItemCodeDescription] public int SeedItemCode;
    public int[] GrowthDays;
    public int TotalGrowthDays;
    public GameObject[] GrowthPrefab;
    public Sprite[] GrowthSprites;
    public Season[] Seasons;
    public Sprite HarvestedSprite;
    [ItemCodeDescription] public int HarvestedTransformItemCode;
    public bool HideCropBeforeHarvestedAnimation;
    public bool DisableCropCollidersBeforeHarvestedAnimation;
    public bool IsHarvestedAnimation;
    public bool IsHarvestActionEffect = false;
    public bool SpawnCropProducedAtPlayerPosition;
    public HarvestActionEffect HarvestActionEffect;

    [ItemCodeDescription] public int[] HarvestToolItemCodes;
    public int[] RequiredHarvestActions;
    [ItemCodeDescription] public int[] CropProducedItemCode;
    public int[] CropProducedMinQuantity;
    public int[] CropProducedMaxQuantity;
    public int DaysToRegrow;

    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < HarvestToolItemCodes.Length; ++i)
        {
            if (HarvestToolItemCodes[i] == toolItemCode)
            {
                return RequiredHarvestActions[i];
            }
        }

        return -1;
    }
}
