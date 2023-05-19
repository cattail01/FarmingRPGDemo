using Enums;
using UnityEngine;

public class AStarTest : MonoBehaviour
{
    [SerializeField] private NPCPath npcPath = null;
    [SerializeField] private bool moveNPC = false;
    [SerializeField] private Vector2Int finishPosition;
    [SerializeField] private AnimationClip idleDownAnimationClip;
    [SerializeField] private AnimationClip eventAnimationClip;
    [SerializeField] private SceneName sceneName = SceneName.Scene1_Farm;
    private NPCMovement npcMovement;

    private void Start()
    {
        npcMovement = npcPath.GetComponent<NPCMovement>();
        npcMovement.NpcFacingDirectionAtDestination = Direction.Down;
        npcMovement.NpcTargetAnimationClip = idleDownAnimationClip;
    }

    private void Update()
    {
        if (moveNPC)
        {
            moveNPC = false;
            NPCScheduleEvent npcScheduleEvent = new NPCScheduleEvent(0, 0, 0, 0, Weather.None, Season.None,
                sceneName, new GridCoordinate(finishPosition.x, finishPosition.y), eventAnimationClip);
            npcPath.BuildPath(npcScheduleEvent);
        }
    }
}

/*
using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AStar))]
public class AStarTest : MonoBehaviour
{
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private Vector2Int finishPosition;
    [SerializeField] private Tilemap tileMapToDisplayPathOn;
    [SerializeField] private TileBase tileToUseToDisplayPath;
    [SerializeField] private bool displayStartAndFinish;
    [SerializeField] private bool displayPath;

    private AStar aStar;
    private Stack<NPCMovementStep> npcMovementSteps;

    private void Awake()
    {
        aStar = GetComponent<AStar>();
        npcMovementSteps = new Stack<NPCMovementStep>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (startPosition != null && finishPosition != null && tileMapToDisplayPathOn != null &&
            tileToUseToDisplayPath != null)
        {
            if (displayStartAndFinish)
            {
                tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0),
                    tileToUseToDisplayPath);
                tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0),
                    tileToUseToDisplayPath);
            }
            else
            {
                tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), null);
                tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), null);
            }

            if (displayPath)
            {
                Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, out SceneName sceneName);
                aStar.BuildPath(sceneName, startPosition, finishPosition, npcMovementSteps);

                foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
                {
                    tileMapToDisplayPathOn.SetTile(
                        new Vector3Int(npcMovementStep.GridCoordinate.x, npcMovementStep.GridCoordinate.y, 0),
                        tileToUseToDisplayPath);
                }
            }
            else
            {
                if (npcMovementSteps.Count > 0)
                {
                    foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
                    {
                        tileMapToDisplayPathOn.SetTile(
                            new Vector3Int(npcMovementStep.GridCoordinate.x, npcMovementStep.GridCoordinate.y, 0),
                            null);
                    }
                    npcMovementSteps.Clear();
                }
            }
        }
    }

}
*/