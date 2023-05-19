using System.Collections.Generic;
using Enums;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonoBehavior<NPCManager>
{
    [SerializeField] private SO_SceneRouteList soSceneRouteList;

    [HideInInspector] public NPC[] npcArray;

    private AStar aStar;
    private Dictionary<string, SceneRoute> sceneRouteDictionary;

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

    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        SceneRoute sceneRoute = null;
        if (sceneRouteDictionary.TryGetValue(fromSceneName + toSceneName, out sceneRoute))
        {
            return sceneRoute;
        }
        else
        {
            return null;
        }
    }

    private void AfterSceneLoad()
    {
        SetNPCsActiveStatus();
    }

    protected override void Awake()
    {
        base.Awake();

        sceneRouteDictionary = new Dictionary<string, SceneRoute>();

        if (soSceneRouteList.SceneRouteList.Count > 0)
        {
            foreach (SceneRoute soSceneRoute in soSceneRouteList.SceneRouteList)
            {
                if (sceneRouteDictionary.ContainsKey(soSceneRoute.FromSceneName.ToString() +
                                                     soSceneRoute.ToSceneName.ToString()))
                {
                    Debug.Log(
                        "** Duplicate Scene Route Key Found ** Check for duplicate routes in the scriptable object scene route list");
                    continue;
                }

                sceneRouteDictionary.Add(soSceneRoute.FromSceneName.ToString() + soSceneRoute.ToSceneName.ToString(),
                    soSceneRoute);
            }
        }

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
