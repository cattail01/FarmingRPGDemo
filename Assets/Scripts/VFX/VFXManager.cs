using Enums;
using System.Collections;
using UnityEngine;

/// <summary>
/// 粒子管理器
/// </summary>
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
    [SerializeField] private GameObject choppingTreeTrunkPrefab;

    /// <summary>
    /// 云山树砍树掉松果粒子预制件
    /// </summary>
    [SerializeField] private GameObject pineConesFallingPrefab;

    private WaitForSeconds twoSeconds;

    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
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
                choppingTreeTrunk.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));
                break;

            case HarvestActionEffect.PineConesFalling:
                GameObject pineConesFalling =
                    PoolManager.Instance.ReuseObject(pineConesFallingPrefab, effectPosition, Quaternion.identity);
                //print(pineConesFallingPrefab.GetInstanceID() + "1");
                pineConesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(pineConesFalling, twoSeconds));
                break;

            case HarvestActionEffect.None:
                break;

            default:
                break;
        }
    }

    #region 脚本声明周期函数

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

    #endregion 脚本声明周期函数
}
