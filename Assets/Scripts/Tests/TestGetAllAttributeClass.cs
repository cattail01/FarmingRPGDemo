using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 测试：获取被某个attribute标记过的所有类
/// </summary>

public class TestGetAllAttributeClass : MonoBehaviour
{
    private void Start()
    {

    }

    private void testGetAllClassByAttribute(Type attributeType)
    {
        // 根据attribute type 获取程序集
        Assembly asm = Assembly.GetAssembly(attributeType);
    }
}