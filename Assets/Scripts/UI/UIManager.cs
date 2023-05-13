using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI管理器
/// </summary>
public class UIManager : SingletonMonoBehavior<UIManager>
{
    // 暂停菜单父物体
    [SerializeField] private GameObject pauseMenu;

    // 菜单中所有页面
    [SerializeField] private GameObject[] menuTabs;

    // 控制菜单页面的所有按钮
    [SerializeField] private Button[] menuButtons;

    // ui 下面的物品栏
    [SerializeField] private UIInventoryBar uiInventoryBar;

    // 暂停菜单仓库管理
    [SerializeField] private PauseMenuInventoryManagement pausedMenuInventoryManagement;

    // 暂停菜单激活状态
    private bool _pauseMenuOn;

    /// <summary>
    /// 暂停菜单激活状态
    /// </summary>
    public bool PauseMenuOn
    {
        get => _pauseMenuOn;
        set => _pauseMenuOn = value;
    }

    /// <summary>
    /// 将按钮颜色设置为激活状态
    /// </summary>
    private void SetButtonColorToActive(Button button)
    {
        // 获取按钮中的颜色struct
        ColorBlock colors = button.colors;

        // 将按钮的正常颜色设置为启用的颜色
        colors.normalColor = colors.pressedColor;

        // 将colors交还
        button.colors = colors;
    }

    /// <summary>
    /// 将按钮颜色设置为不激活的状态
    /// </summary>
    private void SetButtonColorToInactive(Button button)
    {
        // 获取按钮中的颜色struct
        ColorBlock colors = button.colors;

        // 将按钮的正常颜色设置为禁用的颜色
        colors.normalColor = colors.disabledColor;

        // 将颜色struct交给button的colors
        button.colors = colors;
    }

    /// <summary>
    /// 对选中的tab对应的button进行高亮处理
    /// </summary>
    private void HighLightButtonForSelectedTab()
    {
        // 对每一个tab进行遍历
        for (int i = 0; i < menuTabs.Length; ++i)
        {
            // 如果tab显示
            if (menuTabs[i].activeSelf)
            {
                // 将对应的按钮高亮
                SetButtonColorToActive(menuButtons[i]);
            }
            // 如果tab隐藏
            else
            {
                // 将对应的按钮取消高亮
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }

    /// <summary>
    /// 切换暂停菜单页面
    /// </summary>
    public void SwitchPauseMenuTab(int tabNum)
    {
        // 遍历所有的页面
        for (int i = 0; i < menuTabs.Length; ++i)
        {
            // 如果该页面不是目标页面
            if (i != tabNum)
            {
                // 关闭该页面
                menuTabs[i].SetActive(false);
            }
            // 如果该页面是目标页面
            else
            {
                // 打开该页面
                menuTabs[i].SetActive(true);
            }
        }

        // 将页面对应的按钮高亮
        HighLightButtonForSelectedTab();
    }

    /// <summary>
    /// 启用暂停菜单
    /// </summary>
    private void EnablePauseMenu()
    {
        uiInventoryBar.DestroyCurrentlyDraggedItems();

        uiInventoryBar.ClearCurrentlySelectedItems();

        // 设置暂停菜单开启标志为开启
        PauseMenuOn = true;
        // 禁用输入功能
        PlayerSingletonMonoBehavior.Instance.PlayerInputIsDisable = true;
        // 游戏时间暂停
        Time.timeScale = 0f;
        // 将暂停菜单的父物体激活
        pauseMenu.SetActive(true);

        // 趁游戏不注意，触发垃圾回收器
        System.GC.Collect();

        // 高亮选中的按钮
        HighLightButtonForSelectedTab();
    }

    /// <summary>
    /// 禁用暂停菜单
    /// </summary>
    public void DisablePauseMenu()
    {
        pausedMenuInventoryManagement.DestroyCurrentlyDraggedItems();

        // 暂停菜单设置状态设置为关闭
        PauseMenuOn = false;
        // 启用输入
        PlayerSingletonMonoBehavior.Instance.PlayerInputIsDisable = false;
        // 恢复游戏流速
        Time.timeScale = 1f;
        // 设置暂停菜单激活状态为false
        pauseMenu.SetActive(false);
    }

    /// <summary>
    /// 暂停菜单主功能
    /// </summary>
    private void PauseMenu()
    {
        // 如果按下esc键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 如果暂停菜单启用，则禁用
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            // 否则启用
            else
            {
                EnablePauseMenu();
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }
}
