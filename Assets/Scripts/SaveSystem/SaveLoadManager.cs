using System.Collections;
using System.Collections.Generic;
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
    // 定义可保存对象的列表
    public List<ISaveable> SaveableObjectList;

    // 初始化
    protected override void Awake()
    {
        base.Awake();

        SaveableObjectList = new List<ISaveable>();
    }

    // 定义保存当前场景的方法
    public void StoreCurrentSceneData()
    {
        foreach (ISaveable saveableObject in SaveableObjectList)
        {
            saveableObject.SaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    // 定义解除存储当前场景的方法
    public void RestoreCurrentSceneData()
    {
        foreach (ISaveable saveableObject in SaveableObjectList)
        {
            saveableObject.SaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
