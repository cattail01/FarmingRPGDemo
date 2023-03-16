using System;


/// <summary>
/// Scene Item Massage Class
/// </summary>
/// <remarks>
/// <para>serializable</para>
/// <para>for remain item massage in scene</para>
/// </remarks>
[Serializable]
public class SceneItem
{
    // define code of this item
    public int ItemCode;

    // define vector 3 serializable class
    // for remain position of this item in scene
    public Vector3Serializable Position;

    // item name
    public string ItemName;

    // initialize
    public SceneItem()
    {
        Position = new Vector3Serializable();
    }
}
