using Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonoBehavior<GridPropertiesManager>, ISaveable
{

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

    private bool isFirstTimeSceneLoaded = true;

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

        sceneSave.BoolDictionary = new Dictionary<string, bool>();
        sceneSave.BoolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

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

        if (sceneSave.NameToGridPropertyDetailsDic != null)
        {
            nameToGridPropertyDetailsDic = sceneSave.NameToGridPropertyDetailsDic;
        }

        if (sceneSave.BoolDictionary != null &&
            sceneSave.BoolDictionary.TryGetValue("isFirstTimeSceneLoaded", out bool storedIsFirstTimeSceneLoaded))
        {
            isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
        }

        if (isFirstTimeSceneLoaded)
        {
            EventHandler.CallInstantiateCropPrefabsEvent();
        }

        if (nameToGridPropertyDetailsDic.Count > 0)
        {
            ClearDisplayGridPropertyDetails();
            DisplayGridPropertyDetails();
        }

        if (isFirstTimeSceneLoaded == true)
        {
            isFirstTimeSceneLoaded = false;
        }
    }

    public void SaveableLoad(GameSave gameSave)
    {
        GameObjectSave = gameObjectSave;
        SaveableRestoreScene(SceneManager.GetActiveScene().name);
    }

    public GameObjectSave SaveableSave()
    {
        SaveableStoreScene(SceneManager.GetActiveScene().name);
        return GameObjectSave;
    }

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

        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        SaveableUnregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;

        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    // 定义均匀绘制点和线布局的对象
    private Grid grid;

    // 定义存储绘制点线参数的数组
    [SerializeField] private SO_GridProperties[] gridPropertiesArray;

    // 定义 名称 到 地皮参数细节类 的字典
    private Dictionary<string, GridPropertyDetails> nameToGridPropertyDetailsDic;

    // 定义场景加载后的事件函数
    private void AfterSceneLoaded()
    {
        // 获取 grid 游戏对象
        grid = GameObject.FindObjectOfType<Grid>();

        // 获取 tile map
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
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

            sceneSave.BoolDictionary = new Dictionary<string, bool>();
            sceneSave.BoolDictionary.Add("isFirstTimeSceneLoaded", true);

            GameObjectSave.sceneData_SceneNameToSceneSave.Add(gridProperties.SceneName.ToString(), sceneSave);
        }
    }

    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;

    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // dug
        if (gridPropertyDetails.DaysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    public void DisplayWaterGround(GridPropertyDetails gridPropertyDetails)
    {
        // watered
        if (gridPropertyDetails.DaysSinceWatered > -1)
        {
            ConnectWaterGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile dugTile0 = SetDugTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY), dugTile0);

        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails =
            GetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.DaysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1, 0),
                dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.DaysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1, 0),
                dugTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.DaysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY, 0),
                dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.DaysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY, 0),
                dugTile4);
        }
    }

    private void ConnectWaterGround(GridPropertyDetails gridPropertyDetails)
    {
        Tile wateredTile0 = SetWateredTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY, 0),
            wateredTile0);

        GridPropertyDetails adjacentGridPropertyDetails;

        // grid x, grid y + 1
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.DaysSinceWatered > -1)
        {
            Tile wateredTile1 = SetWateredTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1, 0),
                wateredTile1);
        }

        // grid x, grid y - 1
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.DaysSinceWatered > -1)
        {
            Tile wateredTile2 = SetWateredTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1, 0),
                wateredTile2);
        }

        // grid x - 1, grid y
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.DaysSinceWatered > -1)
        {
            Tile wateredTile3 = SetWateredTile(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY, 0),
                wateredTile3);
        }

        // grid x + 1, grid y
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.DaysSinceWatered > -1)
        {
            Tile wateredTile4 = SetWateredTile(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY, 0),
                wateredTile4);
        }
    }

    private Tile SetDugTile(int xGrid, int yGrid)
    {
        //return null;

        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);

        #region 根据周围的瓷砖是否挖好来设置合适的瓷砖

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }

        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }

        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];

        }

        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];

        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }

        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];

        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];

        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }
        return null;
        #endregion 根据周围的瓷砖是否挖好来设置合适的瓷砖
    }

    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        bool upWatered = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downWatered = IsGridSquareWatered(xGrid, yGrid - 1);
        bool leftWatered = IsGridSquareWatered(xGrid - 1, yGrid);
        bool rightWatered = IsGridSquareWatered(xGrid + 1, yGrid);

        #region 根据周围是否浇水来设置合适的瓷砖

        //if (upWatered && downWatered && rightWatered && leftWatered)
        //{
        //    return wateredGround[];
        //}

        if (!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[0];
        }

        else if (!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[1];
        }

        else if (!upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[2];
        }

        else if (!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[3];
        }

        else if (!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[4];
        }

        else if (upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[5];
        }

        else if (upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[6];
        }

        else if (upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[7];
        }

        else if (upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[8];
        }

        else if (upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[9];
        }

        else if (upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[10];
        }

        else if (upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[11];
        }

        else if (upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[12];
        }

        else if (!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[13];
        }

        else if (!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[14];
        }

        else if (!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[15];
        }

        return null;

        #endregion
    }

    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);
        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.DaysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }

        if (gridPropertyDetails.DaysSinceWatered <= -1)
        {
            return false;
        }

        return true;
    }

    private void DisplayGridPropertyDetails()
    {
        foreach (KeyValuePair<string, GridPropertyDetails> item in nameToGridPropertyDetailsDic)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);

            DisplayWaterGround(gridPropertyDetails);

            DisplayPlantedCrop(gridPropertyDetails);
        }
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();

        ClearDisplayAllPlantedCrops();
    }

    private void ClearDisplayGroundDecorations()
    {
        // 移除地面覆盖层
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour,
        int gameMinute, int gameSecond)
    {
        ClearDisplayGridPropertyDetails();

        foreach (SO_GridProperties gridProperties in gridPropertiesArray)
        {
            if (GameObjectSave.sceneData_SceneNameToSceneSave.TryGetValue(gridProperties.SceneName.ToString(),
                    out SceneSave sceneSave))
            {
                if (sceneSave.NameToGridPropertyDetailsDic != null)
                {
                    for (int i = sceneSave.NameToGridPropertyDetailsDic.Count - 1; i >= 0; --i)
                    {
                        KeyValuePair<string, GridPropertyDetails> items =
                            sceneSave.NameToGridPropertyDetailsDic.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = items.Value;

                        if (gridPropertyDetails.GrowthDays > -1)
                        {
                            gridPropertyDetails.GrowthDays += 1;
                        }

                        if (gridPropertyDetails.DaysSinceWatered > -1)
                        {
                            gridPropertyDetails.DaysSinceWatered = -1;
                        }

                        SetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY,
                            gridPropertyDetails, sceneSave.NameToGridPropertyDetailsDic);
                    }
                }
            }
        }
        DisplayGridPropertyDetails();
    }

    private Transform cropParentTransform;
    [SerializeField] private SO_CropDetailsList so_CropDetailsList;

    private void ClearDisplayAllPlantedCrops()
    {
        Crop[] cropArray = FindObjectsOfType<Crop>();

        foreach (Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }

    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.SeedItemCode <= -1)
        {
            return;
        }

        CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.SeedItemCode);

        if (cropDetails == null)
        {
            return;
        }

        GameObject cropPrefab;

        int growthStages = cropDetails.GrowthDays.Length;

        int currentGrowthStage = 0;
        //int daysCounter = cropDetails.TotalGrowthDays;

        for (int i = growthStages - 1; i >= 0; --i)
        {
            //if (gridPropertyDetails.GrowthDays >= daysCounter)
            if (gridPropertyDetails.GrowthDays >= cropDetails.GrowthDays[i])
            {
                currentGrowthStage = i;
                break;
            }
            //daysCounter = daysCounter - cropDetails.GrowthDays[i];
        }

        cropPrefab = cropDetails.GrowthPrefab[currentGrowthStage];

        Sprite growthSprite = cropDetails.GrowthSprites[currentGrowthStage];

        Vector3 worldPosition =
            groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY, 0));
        worldPosition = new Vector3(worldPosition.x + Settings.GridCellSize / 2, worldPosition.y, worldPosition.z);
        GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

        cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
        cropInstance.transform.SetParent(cropParentTransform);
        cropInstance.GetComponent<Crop>().CropGridPosition =
            new Vector2Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY);
    }

    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition =
            grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

        Crop crop = null;

        for (int i = 0; i < collider2DArray.Length; ++i)
        {
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.CropGridPosition ==
                new Vector2Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY))
            {
                break;
            }

            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.CropGridPosition ==
                new Vector2Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY))
            {
                break;
            }
        }
        return crop;
    }

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return so_CropDetailsList.GetCropDetails(seedItemCode);
    }

}
