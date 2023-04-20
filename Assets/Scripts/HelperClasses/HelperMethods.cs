
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于存放帮助方法
/// </summary>
public static class HelperMethods
{
    /// <summary>
    /// 找出盒子内所有的碰撞器，并根据所有碰撞器物体找到相应组件
    /// </summary>
    /// <typeparam name="T">目标组件的类类型</typeparam>
    /// <param name="listComponentsAtBoxPosition">含有目标组件的列表</param>
    /// <param name="point"></param>
    /// <param name="size"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static bool TryGetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point,
        Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        // 找到盒子内的所有碰撞器
        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        // 对每个碰撞器进行遍历，试图找到目标
        for (int i = 0; i < collider2DArray.Length; ++i)
        {
            T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        listComponentsAtBoxPosition = componentList;

        // 返回是否找到成功
        return found;
    }

    public static bool GetComponentsAtCursorLocation<T>(out List<T> componentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;
        List<T> componentList = new List<T>();
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);

        T tComponent = default(T);
        for (int i = 0; i < collider2DArray.Length; ++i)
        {
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        componentsAtPositionList = componentList;

        return found;
    }

    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size,
        float angle)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];
        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);
        T tComponent = default(T);
        T[] componentArray = new T[collider2DArray.Length];

        for (int i = collider2DArray.Length - 1; i >= 0; --i)
        {
            if (collider2DArray[i] != null)
            {
                tComponent = collider2DArray[i].gameObject.GetComponent<T>();
                if (tComponent != null)
                {
                    componentArray[i] = tComponent;
                }
            }
        }

        return componentArray;
    }

}
