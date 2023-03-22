using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 场景物品管理器
/// </summary>
[RequireComponent(typeof(GenerateGUID))]
public class SceneItemManager : SingletonMonoBehavior<SceneItemManager>, ISaveable
{
    #region 脚本生命周期

    protected override void Awake()
    {
        base.Awake();

        SaveableUniqueId = GetComponent<GenerateGUID>().GUID;
        gameObjectSave = new GameObjectSave();
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

    public string SaveableUniqueId
    {
        get => saveableUniqueId;
        set => saveableUniqueId = value;
    }

    // 定义物品存储器
    private GameObjectSave gameObjectSave;

    public GameObjectSave GameObjectSave
    {
        get => gameObjectSave;
        set => gameObjectSave = value;
    }

    public void SaveableRegister()
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }


    #endregion ISaveable 的部分

    // 定义父物体，用于存放所有的item
    private Transform parentItem;

    // 定义item预制件
    [SerializeField]
    private GameObject itemPrefab;

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
