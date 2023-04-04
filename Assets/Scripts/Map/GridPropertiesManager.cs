using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonoBehavior<GridPropertiesManager>, ISaveable
{

    #region ISaveable 接口的部分

    public string saveableUniqueId;

    public string SaveableUniqueId
    {
        get => saveableUniqueId;
        set => saveableUniqueId = value;
    }

    public GameObjectSave gameObjectSave;

    public GameObjectSave GameObjectSave
    {
        get => gameObjectSave;
        set => gameObjectSave = value;
    }

    public void SaveableRegister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Add(this);
    }

    public void SaveableUnregister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Remove(this);
    }

    public void SaveableStoreScene(string sceneName)
    {
        throw new System.NotImplementedException();
    }

    public void SaveableRestoreScene(string sceneName)
    {
        SceneSave sceneSave;
        if (!GameObjectSave.sceneData_SceneNameToSceneSave.TryGetValue(sceneName, out sceneSave))
        {
            return;
        }

        if (sceneSave.NameToGridPropertyDetailsDic == null)
        {
            return;
        }

        nameToGridPropertyDetailsDic = sceneSave.NameToGridPropertyDetailsDic;
    }

    #endregion ISaveable 接口的部分

    #region 脚本生命周期

    protected override void Awake()
    {
        base.Awake();

        saveableUniqueId = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = GetComponent<GameObjectSave>();
    }

    private void OnEnable()
    {
        SaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;

    }

    private void OnDisable()
    {
        SaveableUnregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    #endregion 脚本生命周期

    // 定义均匀绘制点和线布局的对象
    public Grid Grid;

    // 定义存储绘制点线参数的数组
    [SerializeField] private SO_GridProperties[] gridPropertiesArray;

    // 定义 名称 到 地皮参数细节类 的字典
    private Dictionary<string, GridPropertyDetails> nameToGridPropertyDetailsDic;

    // 定义场景加载后的事件函数
    private void AfterSceneLoaded()
    {
        // 获取 grid 游戏对象
        Grid = GameObject.FindObjectOfType<Grid>();
    }

    // 为所提供的字典在gridlocation处转换gridPropertyDetails，如果location.il上不存在属性则为null
    // 定义查找 GridPropertyDetails 的方法
    // 通过gridX和gridY，找到对应的cell 参数细节信息类
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY,
        Dictionary<string, GridPropertyDetails> gridPropertyDic)
    {
        // 为相应的cell构建 string key
        string key = "x" + gridX + "y" + gridY;

        // 定义cell参数细节
        GridPropertyDetails gridPropertyDetails;

        // 如果找不到相关的参数细节信息，返回null
        if (!gridPropertyDic.TryGetValue(key, out gridPropertyDetails))
        {
            return null;
        }

        // 返回找到的 cell 参数细节
        return gridPropertyDetails;
    }

    // 通过gridX和gridY，找到对应的cell 参数细节信息类
    // 重载：
    // GetGridPropertyDetails(int gridX, int gridY,
    // Dictionary<string, GridPropertyDetails> gridPropertyDic)
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, nameToGridPropertyDetailsDic);
    }

    // 定义 初始化 cell 参数 的方法
    private void InitialiseGridProperties()
    {
        // 对所有的 SO_GridProperties 可脚本化类进行遍历
        // 对于每一个 SO_GridProperties 实例化对象：
        foreach (SO_GridProperties gridProperties in gridPropertiesArray)
        {
            
        }
    }
}
