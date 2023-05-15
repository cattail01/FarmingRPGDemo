using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 保存加载控制器
/// </summary>
/// <remarks>
/// <para>单例unity脚本</para>
/// <para>控制器，用于控制场景加载时的复现与场景卸载时的保存</para>
/// </remarks>
public class SaveLoadManager : SingletonMonoBehavior<SaveLoadManager>
{
    /// <summary>
    /// 定义可保存对象的列表
    /// </summary>
    public List<ISaveable> SaveableObjectList;

    public  GameSave GameSave;

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        SaveableObjectList = new List<ISaveable>();
    }

    /// <summary>
    /// 定义保存当前场景的方法
    /// </summary>
    public void StoreCurrentSceneData()
    {
        foreach (ISaveable saveableObject in SaveableObjectList)
        {
            saveableObject.SaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// 定义解除存储当前场景的方法
    /// </summary>
    public void RestoreCurrentSceneData()
    {
        foreach (ISaveable saveableObject in SaveableObjectList)
        {
            saveableObject.SaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            GameSave = new GameSave();

            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);
            
            GameSave = bf.Deserialize(file) as GameSave;

            for (int i = SaveableObjectList.Count - 1; i > -1; --i)
            {
                if (GameSave.GameObjectData.ContainsKey(SaveableObjectList[i].SaveableUniqueId))
                {
                    SaveableObjectList[i].SaveableLoad(GameSave);
                }
                else
                {
                    Component component = SaveableObjectList[i] as Component;

                    Destroy(component.gameObject);
                }
            }
            file.Close();

        }
        UIManager.Instance.DisablePauseMenu();

    }

    public void SaveDataToFile()
    {
        GameSave = new GameSave();
        foreach (ISaveable SaveableObject in SaveableObjectList)
        {
            GameSave.GameObjectData.Add(SaveableObject.SaveableUniqueId, SaveableObject.SaveableSave());
        }

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);

        bf.Serialize(file, GameSave);

        file.Close();

        UIManager.Instance.DisablePauseMenu();
    }
}
