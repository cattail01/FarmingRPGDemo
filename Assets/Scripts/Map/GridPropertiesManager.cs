using Enums;
using System.Collections.Generic;
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
        // 删除场景保存的信息
        GameObjectSave.sceneData_SceneNameToSceneSave.Remove(sceneName);

        // 为场景创建场景保存信息
        SceneSave sceneSave = new SceneSave();

        // 创建并添加 dict grid 参数细节类 字典
        sceneSave.NameToGridPropertyDetailsDic = nameToGridPropertyDetailsDic;

        // 将 scene save 类 添加到 game object save 类中
        GameObjectSave.sceneData_SceneNameToSceneSave.Add(sceneName, sceneSave);
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

        SaveableUniqueId = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
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

    // 通过 grid x、y 向dictionary中添加 GridPropertyDetails
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails,
        Dictionary<string, GridPropertyDetails> stringToGridPropertyDetailsDic)
    {
        // 构造key
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.GridX = gridX;
        gridPropertyDetails.GridY = gridY;

        // 设置值
        stringToGridPropertyDetailsDic[key] = gridPropertyDetails;
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, nameToGridPropertyDetailsDic);
    }

    // 定义 初始化 cell 参数 的方法
    private void InitialiseGridProperties()
    {
        // 对所有的 SO_GridProperties 可脚本化类进行遍历
        // 对于每一个 SO_GridProperties 实例化对象：
        foreach (SO_GridProperties gridProperties in gridPropertiesArray)
        {
            // 为 单元参数细节类 创建字典
            Dictionary<string, GridPropertyDetails> nameToGridPropertyDetailsDic =
                new Dictionary<string, GridPropertyDetails>();

            // 对 scriptable object gridProperties 中记录的所有方块参数的信息进行遍历
            foreach (GridProperty gridProperty in gridProperties.GridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;

                // 根据位置和字典获取参数信息
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.GridCoordinate.x,
                    gridProperty.GridCoordinate.y, nameToGridPropertyDetailsDic);

                // 如果 gridPropertyDetails 为空，则创建
                //if (gridPropertyDetails == null)
                //{
                //    gridPropertyDetails = new GridPropertyDetails();
                //}
                gridPropertyDetails ??= new GridPropertyDetails();

                // 
                switch (gridProperty.GridBoolProperty)
                {
                    case GridBoolProperty.Diggable:
                        gridPropertyDetails.IsDiggable = gridProperty.GridBoolValue;
                        break;
                    case GridBoolProperty.CanDropItem:
                        gridPropertyDetails.CanDropItem = gridProperty.GridBoolValue;
                        break;
                    case GridBoolProperty.CanPlaceFurniture:
                        gridPropertyDetails.CanPlaceFurniture = gridProperty.GridBoolValue;
                        break;
                    case GridBoolProperty.IsPath:
                        gridPropertyDetails.IsPath = gridProperty.GridBoolValue;
                        break;
                    case GridBoolProperty.IsNpcObstacle:
                        gridPropertyDetails.IsNpcObstacle = gridProperty.GridBoolValue;
                        break;
                    default:
                        break;
                }

                SetGridPropertyDetails(gridProperty.GridCoordinate.x, gridProperty.GridCoordinate.y,
                    gridPropertyDetails, nameToGridPropertyDetailsDic);
            }
            SceneSave sceneSave = new SceneSave();

            sceneSave.NameToGridPropertyDetailsDic = nameToGridPropertyDetailsDic;

            if (gridProperties.SceneName.ToString() == SceneControllerManager.Instance.StartingSceneName.ToString())
            {
                this.nameToGridPropertyDetailsDic = nameToGridPropertyDetailsDic;
            }

            GameObjectSave.sceneData_SceneNameToSceneSave.Add(gridProperties.SceneName.ToString(), sceneSave);
        }
    }
}
