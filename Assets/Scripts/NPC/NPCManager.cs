using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonoBehavior<NPCManager>
{
    [HideInInspector] public NPC[] npcArray;

    private AStar aStar;

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition,
        Stack<NPCMovementStep> npcMovementStepStack)
    {
        if (aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetNPCsActiveStatus()
    {
        foreach (NPC npc in npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();

            if (npcMovement.NpcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }

    private void AfterSceneLoad()
    {
        SetNPCsActiveStatus();
    }

    protected override void Awake()
    {
        base.Awake();

        aStar = GetComponent<AStar>();
        npcArray = FindObjectsOfType<NPC>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }
}
