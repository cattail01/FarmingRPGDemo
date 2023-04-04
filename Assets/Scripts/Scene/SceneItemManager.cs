using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景物品管理器
/// </summary>
/// <remarks>
/// <para>继承SingletonMonoBehavior脚本</para>
/// <para>需要GenerateGUID组件，用于提供单一编号</para>
/// </remarks>
[RequireComponent(typeof(GenerateGUID))]
public class SceneItemManager : SingletonMonoBehavior<SceneItemManager>, ISaveable
{
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
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        SaveableUnregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    #endregion 脚本生命周期

    #region ISaveable 的部分

    // 定义标识存储的独一无二的id
    private string saveableUniqueId;

    // 定义标识存储的独一无二的id的访问器
    // 实现ISaveable接口
    public string SaveableUniqueId
    {
        get => saveableUniqueId;
        set => saveableUniqueId = value;
    }

    // 定义物品存储器
    private GameObjectSave gameObjectSave;

    // 定义物品存储器的访问器
    // 实现ISaveable接口
    public GameObjectSave GameObjectSave
    {
        get => gameObjectSave;
        set => gameObjectSave = value;
    }

    // 实现ISaveable接口
    // 定义存储注册函数
    public void SaveableRegister()
    {
        // 将该saveable控制器添加到控制器中的SaveableObjectList中
        SaveLoadManager.Instance.SaveableObjectList.Add(this);
    }

    // 实现ISaveable接口
    // 定义存储解注册的函数
    public void SaveableUnregister()
    {
        // 将该saveable从控制器的SaveableObjectList中移除
        SaveLoadManager.Instance.SaveableObjectList.Remove(this);
    }

    // 保存场景中所有物体
    public void SaveableStoreScene(string sceneName)
    {
        // 当该sceneName被记录在gameobjectsave的scene data中时，因为它是旧的场景保存，所以需要删除
        GameObjectSave.sceneData_SceneNameToSceneSave.Remove(sceneName);

        // 从场景中获取所有的物体
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        // 遍历所有场景中的物体，将每个物体的信息记录下来并存储
        foreach (Item item in itemsInScene)
        {
            // 创建存储item信息的容器，并记录信息
            SceneItem sceneItem = new SceneItem();
            sceneItem.ItemCode = item.ItemCode;
            sceneItem.Position = new Vector3Serializable(item.transform.position.x, item.transform.position.y,
                item.transform.position.z);
            sceneItem.ItemName = item.name;

            // 将该sceneitem信息存储到list中
            sceneItemList.Add(sceneItem);
        }

        // 创建SceneSave对象，创建字典
        SceneSave sceneSave = new SceneSave();
        //sceneSave.NameToSceneItemListDictionary = new Dictionary<string, List<SceneItem>>();
        //sceneSave.NameToSceneItemListDictionary.Add("sceneItemList", sceneItemList);
        sceneSave.SceneItemList = sceneItemList;

        // 将场景保存添加到 game object save
        GameObjectSave.sceneData_SceneNameToSceneSave.Add(sceneName, sceneSave);
    }

    // 取消存储场景中的所有物体
    public void SaveableRestoreScene(string sceneName)
    {
        SceneSave sceneSave;
        if (!GameObjectSave.sceneData_SceneNameToSceneSave.TryGetValue(sceneName, out sceneSave))
        {
            return;
        }

        if (sceneSave.SceneItemList != null)
        {
            // 销毁场景中的所有物体
            DestroySceneItem();

            // 按照SceneItemList创建物体
            InstantiateSceneItems(sceneSave.SceneItemList);
        }
    }

    #endregion ISaveable 的部分

    // 定义父物体，用于存放所有的item
    private Transform parentItem;

    // 定义item预制件
    [SerializeField] private GameObject itemPrefab;

    // 定义在场景加载完毕后执行的函数
    public void AfterSceneLoad()
    {
        // 获取父物体
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).GetComponent<Transform>();
    }

    // 定义销毁场景中所有物体的方法
    private void DestroySceneItem()
    {
        // 通过item类型，获取场景中所有物体
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        // 递归循环销毁所有物体
        for (int i = itemsInScene.Length - 1; i > -1; --i)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    // 定义创建item的方法
    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        // 创建物体
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);

        // 获取物体的item组件
        Item item = itemGameObject.GetComponent<Item>();

        // 初始化该item
        item.Init(itemCode);
    }

    // 定义批量创建场景物体的方法
    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        // 定义item游戏对象
        GameObject itemGameObject;

        // 对场景列表中的物体进行遍历，创建并设置相应item物体
        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab,
                new Vector3(sceneItem.Position.x, sceneItem.Position.y, sceneItem.Position.z), Quaternion.identity,
                parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.ItemCode;
            item.name = sceneItem.ItemName;
        }
    }
}
