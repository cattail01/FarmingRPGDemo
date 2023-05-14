using System;
using System.Collections.Generic;

[Serializable]
public class GameSave
{
    public Dictionary<string, GameObjectSave> GameObjectData;

    public GameSave()
    {
        GameObjectData = new Dictionary<string, GameObjectSave>();
    }
}