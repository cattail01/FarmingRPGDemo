using Enums;
using System.Collections;
using UnityEngine;

public class Crop : MonoBehaviour
{
    #region 属性

    #region SerializeField private

    /// <summary>
    /// 收割作物的图片
    /// </summary>
    [Tooltip("使用子游戏对象填充")]
    [SerializeField]
    private SpriteRenderer cropHarvestedSpriteRenderer;

    /// <summary>
    /// 收割作物的粒子产生点
    /// </summary>
    [Tooltip("使用子游戏对象填充")]
    [SerializeField] private Transform harvestActionEffectTransform = null;

    #endregion SerializeField private

    /// <summary>
    /// 作物在网格中的位置
    /// </summary>
    [HideInInspector] public Vector2Int CropGridPosition;

    /// <summary>
    /// 收割动作计数器
    /// </summary>
    private int harvestActionCount = 0;

    #endregion 属性

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

    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.SeedItemCode = cropDetails.HarvestedTransformItemCode;
        gridPropertyDetails.GrowthDays = 0;
        gridPropertyDetails.DaysSinceLastHarvest = -1;
        gridPropertyDetails.DaysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY,
            gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);

        if (cropDetails.HarvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }

        Destroy(gameObject);
    }

    private IEnumerator ProcessHarvestActionAfterAnimation(CropDetails cropDetails,
        GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails,
        GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        if (cropDetails.IsHarvestedAnimation && animator != null)
        {
            if (cropDetails.HarvestedSprite != null)
            {
                if (cropHarvestedSpriteRenderer != null)
                {
                    cropHarvestedSpriteRenderer.sprite = cropDetails.HarvestedSprite;
                }
            }

            if (isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestright");
            }
            else
            {
                animator.SetTrigger("harvestleft");
            }
        }

        gridPropertyDetails.SeedItemCode = -1;
        gridPropertyDetails.GrowthDays = -1;
        gridPropertyDetails.DaysSinceLastHarvest = -1;
        gridPropertyDetails.DaysSinceWatered = -1;

        if (cropDetails.HideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        if (cropDetails.DisableCropCollidersBeforeHarvestedAnimation)
        {
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY,
            gridPropertyDetails);

        if (cropDetails.IsHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else
        {
            HarvestActions(cropDetails, gridPropertyDetails);
        }
    }

    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown,
        bool isToolUp)
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

        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        if (cropDetails.IsHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position,
                cropDetails.HarvestActionEffect);
        }

        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.ItemCode);
        if (requiredHarvestActions == -1)
        {
            return;
        }

        harvestActionCount += 1;

        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
        }
    }
}
