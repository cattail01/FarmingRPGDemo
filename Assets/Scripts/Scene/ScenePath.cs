using Enums;
using System;

[Serializable]
public class ScenePath
{
    public SceneName SceneName;
    public GridCoordinate FromGridCell;
    public GridCoordinate ToGridCell;
}
