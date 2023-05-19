using Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(GenerateGUID))]
public class NPC : MonoBehaviour, ISaveable
{
    private string _saveableUniqueId;

    public string SaveableUniqueId
    {
        get => _saveableUniqueId;
        set => _saveableUniqueId = value;
    }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave
    {
        get => _gameObjectSave;
        set => _gameObjectSave = value;
    }

    private NPCMovement npcMovement;

    public void SaveableStoreScene(string sceneName)
    {

    }

    public void SaveableRestoreScene(string sceneName)
    {

    }

    public GameObjectSave SaveableSave()
    {
        GameObjectSave.sceneData_SceneNameToSceneSave.Remove(Settings.PersistentScene);
        SceneSave sceneSave = new SceneSave();
        sceneSave.Vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave.StringDictionary = new Dictionary<string, string>();

        sceneSave.Vector3Dictionary.Add("npcTargetGridPosition",
            new Vector3Serializable(npcMovement.NpcTargetGridPosition.x, npcMovement.NpcTargetGridPosition.y,
                npcMovement.NpcTargetGridPosition.z));
        sceneSave.Vector3Dictionary.Add("npcTargetWorldPosition",
            new Vector3Serializable(npcMovement.NpcTargetWorldPosition.x, npcMovement.NpcTargetWorldPosition.y,
                npcMovement.NpcTargetWorldPosition.z));
        sceneSave.StringDictionary.Add("npcTargetScene", npcMovement.NpcTargetScene.ToString());

        GameObjectSave.sceneData_SceneNameToSceneSave.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void SaveableRegister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Add(this);
    }

    public void SaveableUnregister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Remove(this);
    }

    public void SaveableLoad(GameSave gameSave)
    {
        if (gameSave.GameObjectData.TryGetValue(SaveableUniqueId, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if (GameObjectSave.sceneData_SceneNameToSceneSave.TryGetValue(Settings.PersistentScene,
                    out SceneSave sceneSave))
            {
                if (sceneSave.Vector3Dictionary != null && sceneSave.StringDictionary != null)
                {
                    if (sceneSave.Vector3Dictionary.TryGetValue("npcTargetGridPosition",
                            out Vector3Serializable savedNPCTargetGridPosition))
                    {
                        npcMovement.NpcTargetGridPosition = new Vector3Int((int)savedNPCTargetGridPosition.x,
                            (int)savedNPCTargetGridPosition.y, (int)savedNPCTargetGridPosition.z);
                        npcMovement.NpcCurrentGridPosition = npcMovement.NpcTargetGridPosition;
                    }

                    if (sceneSave.Vector3Dictionary.TryGetValue("npcTargetWorldPosition",
                            out Vector3Serializable savedNPCTargetWorldPosition))
                    {
                        npcMovement.NpcTargetWorldPosition = new Vector3(savedNPCTargetWorldPosition.x,
                            savedNPCTargetWorldPosition.y, savedNPCTargetWorldPosition.z);
                        transform.position = npcMovement.NpcTargetWorldPosition;
                    }

                    if (sceneSave.StringDictionary.TryGetValue("npcTargetScene", out string savedTargetScene))
                    {
                        if (Enum.TryParse<SceneName>(savedTargetScene, out SceneName sceneName))
                        {
                            npcMovement.NpcTargetScene = sceneName;
                            npcMovement.NpcCurrentScene = npcMovement.NpcTargetScene;
                        }
                    }

                    npcMovement.CancelNPCMovement();
                }
            }
        }
    }

    private void Awake()
    {
        SaveableUniqueId = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        SaveableRegister();
    }

    private void OnDisable()
    {
        SaveableUnregister();
    }

    private void Start()
    {
        npcMovement = GetComponent<NPCMovement>();
    }
}
