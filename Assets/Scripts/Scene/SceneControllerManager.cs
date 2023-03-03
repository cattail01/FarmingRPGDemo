using Enums;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;      // for SceneManager
using UnityEngine.UI;       // for Image, CanvasGroup

///<summary>
/// 定义场景控制器类
///</summary>
/// <remarks>
/// 是一个需要挂载到物体上的单例
/// </remarks>
public class SceneControllerManager : SingletonMonoBehavior<SceneControllerManager>
{
	#region 场景管理参数

	// 定义隐藏状态
	private bool IsFading;

	// 定义隐藏的持续时间
	[SerializeField] private float fadeDuration = 1f;

	// 定义用于遮挡scene加载的image的CanvasGroup
	[SerializeField] private CanvasGroup faderCanvasGroup = null;

	// 定义用于遮挡scene加载的Fader Image
	[SerializeField] private Image faderImage = null;

	// 定义场景名称
	public SceneName StartingSceneName;

	#endregion 场景管理参数

	private IEnumerator Start()
	{

		faderImage.color = new Color(0f, 0f, 0f, 1f);

		yield return StartCoroutine(LoadSceneAndSetActive(StartingSceneName.ToString()));

		EventHandler.CallAfterSceneLoadEvent();

		yield return StartCoroutine(FadeImageAlphaTo(0f));
	}

	/// <summary>
	/// 定义切换场景的函数
	/// </summary>
	/// <remarks>
	/// 主要的场景切换入口函数，游戏角色希望切换场景时调用
	/// </remarks>
	/// <param name="sceneName">场景名称</param>
	/// <param name="spawnPosition">游戏角色在下一个场景的位置</param>
	public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
	{
		// 如果场景正在切换
		if (IsFading)
			return;

		// 调用切换场景方法
		StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
	}

	// 定义场景切换的方法
	private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
	{
		// 调用场景卸载之前，黑幕升起的事件
		EventHandler.CallBeforeSceneUnloadFadeOutEvent();

		// 开始显示用于隐藏场景的ui黑幕
		// 直到黑幕完全变黑再进行下面的操作
		yield return StartCoroutine(FadeImageAlphaTo(1f));

		// 设置游戏角色的位置
		PlayerSingletonMonoBehavior.Instance.gameObject.transform.position = spawnPosition;

		// 调用场景卸载前的事件
		EventHandler.CallBeforeSceneUnloadEvent();

		// 将当前active的场景卸载掉
		// 直到卸载完场景后执行下面的操作
		yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

		// 调用场景卸载后的事件
		EventHandler.CallAfterSceneUnloadEvent();

		// 开始加载场景
		// 直到加载完场景执行后面的操作
		yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

		// 调用场景加载结束事件
		EventHandler.CallAfterSceneLoadEvent();

		// 开始将幕布变淡，直到完全消失后执行接下来的代码
		yield return StartCoroutine(FadeImageAlphaTo(0f));

		// 调用场景加载结束后、并且幕布撤下后的时间
		EventHandler.CallAfterSceneLoadFadeInEvent();
	}

	// 定义控制幕布显示隐藏的方法
	private IEnumerator FadeImageAlphaTo(float finalAlpha)
	{
		// 设置幕布正在隐藏
		IsFading = true;

		// 设置隐藏 canvas group 射线阻挡功能 blocks raycasts 开启，用于阻挡射线
		faderCanvasGroup.blocksRaycasts = true;

		// 计算调整alpha值的速度
		float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

		yield return null;

		// 当幕布没有到达指定的透明值
		while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
		{
			// 让幕布alpha值接近目标alpha值
			faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, 
				fadeSpeed * Time.deltaTime);

			// wait for a frame then continue
			yield return null;
		}

		// 设置幕布没有活动
		IsFading = false;

		// 关闭raycast射线遮挡
		faderCanvasGroup.blocksRaycasts = false;
	}

	// 定义加载scene并设置为active场景的方法
	private IEnumerator LoadSceneAndSetActive(string sceneName)
	{
		// 异步加载场景，使用add模式
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

		// 找到新的场景（loaded scene中最后一个加载的场景）
		Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

		// 让新场景设置为active
		SceneManager.SetActiveScene(newlyLoadedScene);

		yield return null;
	}

}
