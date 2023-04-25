using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonoBehavior<PoolManager>
{
    [Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    [SerializeField] private Pool[] pool = null;
    [SerializeField] private Transform objectPoolTransform = null;

    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        for (int i = 0; i < pool.Length; ++i)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;

        // 创建父游戏物体
        // 用于保存创建的多个预制件实例
        GameObject parentGameObject = new GameObject($"{prefabName}Anchor");

        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());

            for (int i = 0; i < poolSize; ++i)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObject(position, rotation, objectToReuse, prefab);
            return objectToReuse;
        }
        else
        {
            Debug.Log($"No object pool for {prefab}");
            return null;
        }
    }

    public GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(objectToReuse);

        if (objectToReuse.activeSelf)
        {
            objectToReuse.SetActive(false);
        }

        return objectToReuse;
    }

    public static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;

        objectToReuse.transform.localScale = prefab.transform.localScale;
    }
}
