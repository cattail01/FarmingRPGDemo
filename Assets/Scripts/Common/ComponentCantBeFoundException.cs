using System;
using UnityEngine;

public class ComponentCantBeFoundException : Exception
{
    //private string massage;
    public string Massage { get; set; }

    public ComponentCantBeFoundException() : base()
    {

    }

    public ComponentCantBeFoundException(string massage) : base(massage)
    {
        Massage = massage;
        Debug.Log(Massage);
    }

    // 主要使用这个构造函数
    public ComponentCantBeFoundException(string gameObjectName, string componentName, string className, string methodName)
    {
        Massage = string.Format("[{0}.{1}]: {2} can't find component {3}", className, methodName, gameObjectName,
            componentName);
        Debug.Log(Massage);
    }
}
