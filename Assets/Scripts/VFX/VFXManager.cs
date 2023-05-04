using Enums;
using System.Collections;
using UnityEngine;

public class VFXManager : SingletonMonoBehavior<VFXManager>
{
    /// <summary>
    /// 收割粒子预制件
    /// </summary>
    [SerializeField] private GameObject reapingPrefab;

    /// <summary>
    /// 落叶粒子预制件
    /// </summary>
    [SerializeField] private GameObject deciduousLeavesFallingPrefab;

    /// <summary>
    /// 砍树桩粒子预制件
    /// </summary>
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;

    private WaitForSeconds twoSeconds;


    protected override void Awake()
    {
        base.Awake();

        twoSeconds = new WaitForSeconds(2f);
    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += DisplayHarvestActionEffect;
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= DisplayHarvestActionEffect;
    }

    private void DisplayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        switch (harvestActionEffect)
        {
            case HarvestActionEffect.Reaping:
                GameObject reaping =
                    PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;

            case HarvestActionEffect.DeciduousLeavesFalling:
                GameObject deciduousLeavesFalling = PoolManager.Instance.ReuseObject(deciduousLeavesFallingPrefab,
                    effectPosition, Quaternion.identity);
                deciduousLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(deciduousLeavesFalling, twoSeconds));
                break;

            case HarvestActionEffect.ChoppingTreeTrunk:
                GameObject choppingTreeTrunk =
                    PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));
                break;

            case HarvestActionEffect.None:
                break;

            default:
                break;
        }
    }

    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }
}
