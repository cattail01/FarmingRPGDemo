using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    public Stack<NPCMovementStep> NpcMovementStepStack;

    private NPCMovement npcMovement;

    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNPCMovementStep)
    {
        if ((npcMovementStep.GridCoordinate.x != previousNPCMovementStep.GridCoordinate.x) &&
            (npcMovementStep.GridCoordinate.y != previousNPCMovementStep.GridCoordinate.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateTimesOnPath()
    {
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        NPCMovementStep previousNpcMovementStep = null;

        foreach (NPCMovementStep npcMovementStep in NpcMovementStepStack)
        {
            if (previousNpcMovementStep == null)
            {
                previousNpcMovementStep = npcMovementStep;
            }

            npcMovementStep.Hour = currentGameTime.Hours;
            npcMovementStep.Minute = currentGameTime.Minutes;
            npcMovementStep.Second = currentGameTime.Seconds;

            TimeSpan movementTimeStep;

            if (MovementIsDiagonal(npcMovementStep, previousNpcMovementStep))
            {
                movementTimeStep = new TimeSpan(0, 0,
                    (int)(Settings.GridCellDiagonalSize / Settings.SecondsPerGameSecond / npcMovement.NpcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0,
                    (int)(Settings.GridCellSize / Settings.SecondsPerGameSecond / npcMovement.NpcNormalSpeed));
            }

            currentGameTime = currentGameTime.Add(movementTimeStep);
            previousNpcMovementStep = npcMovementStep;
        }
    }

    public void ClearPath()
    {
        NpcMovementStepStack.Clear();
    }

    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();

        if (npcScheduleEvent.ToSceneName == npcMovement.NpcCurrentScene)
        {
            Vector2Int npcCurrentGridPosition = (Vector2Int)npcMovement.NpcCurrentGridPosition;
            Vector2Int npcTargetGridPosition = (Vector2Int)npcScheduleEvent.ToGridCoordinate;
            NPCManager.Instance.BuildPath(npcScheduleEvent.ToSceneName, npcCurrentGridPosition, npcTargetGridPosition,
                NpcMovementStepStack);
        }

        else if (npcScheduleEvent.ToSceneName != npcMovement.NpcCurrentScene)
        {
            SceneRoute sceneRoute;
            sceneRoute = NPCManager.Instance.GetSceneRoute(npcMovement.NpcCurrentScene.ToString(),
                npcScheduleEvent.ToSceneName.ToString());
            if (sceneRoute != null)
            {
                for (int i = sceneRoute.ScenePathList.Count - 1; i >= 0; --i)
                {
                    int toGridX, toGridY, fromGridX, fromGridY;
                    ScenePath scenePath = sceneRoute.ScenePathList[i];

                    if (scenePath.ToGridCell.x >= Settings.MaxGridWidth ||
                        scenePath.ToGridCell.y >= Settings.MaxGridHeight)
                    {
                        toGridX = npcScheduleEvent.ToGridCoordinate.x;
                        toGridY = npcScheduleEvent.ToGridCoordinate.y;
                    }
                    else
                    {
                        toGridX = scenePath.ToGridCell.x;
                        toGridY = scenePath.ToGridCell.y;
                    }

                    if (scenePath.FromGridCell.x >= Settings.MaxGridWidth ||
                        scenePath.FromGridCell.y >= Settings.MaxGridHeight)
                    {
                        fromGridX = npcMovement.NpcCurrentGridPosition.x;
                        fromGridY = npcMovement.NpcCurrentGridPosition.y;
                    }

                    else
                    {
                        fromGridX = scenePath.FromGridCell.x;
                        fromGridY = scenePath.FromGridCell.y;
                    }

                    Vector2Int fromGridPosition = new Vector2Int(fromGridX, fromGridY);
                    Vector2Int toGridPosition = new Vector2Int(toGridX, toGridY);

                    NPCManager.Instance.BuildPath(scenePath.SceneName, fromGridPosition, toGridPosition,
                        NpcMovementStepStack);
                }
            }
        }

        if (NpcMovementStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            NpcMovementStepStack.Pop();
            npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }
    }

    private void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
        NpcMovementStepStack = new Stack<NPCMovementStep>();
    }
}
