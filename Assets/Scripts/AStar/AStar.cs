using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Tiles & Tilemap References")] [Header("Options")] [SerializeField]
    private bool observeMovementPenalties = true;

    [Range(0, 20)] [SerializeField] private int pathMovementPenalty;

    [Range(0, 20)] [SerializeField] private int defaultMovementPenalty;

    private GridNodes gridNodes;
    private Node startNode;
    private Node targetNode;
    private int gridWidth;
    private int gridHeight;
    private int originX;
    private int originY;

    private List<Node> openNodeList;
    private HashSet<Node> closedNodeList;

    private bool pathFound = false;

    private bool PopulateGridNodesFromGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition,
        Vector2Int endGridPosition)
    {
        SceneSave sceneSave;

        if (GridPropertiesManager.Instance.GameObjectSave.sceneData_SceneNameToSceneSave.TryGetValue(
                sceneName.ToString(), out sceneSave))
        {
            if (sceneSave.NameToGridPropertyDetailsDic != null)
            {
                if (GridPropertiesManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions,
                        out Vector2Int gridOrigin))
                {
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    openNodeList = new List<Node>();
                    closedNodeList = new HashSet<Node>();
                }

                else
                {
                    return false;
                }

                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x,
                    startGridPosition.y - gridOrigin.y);
                targetNode = gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                for (int x = 0; x < gridDimensions.x; ++x)
                {
                    for (int y = 0; y < gridDimensions.y; ++y)
                    {
                        GridPropertyDetails gridPropertyDetails =
                            GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y,
                                sceneSave.NameToGridPropertyDetailsDic);
                        if (gridPropertyDetails != null)
                        {
                            if (gridPropertyDetails.IsNpcObstacle == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.IsObstacle = true;
                            }
                            else if (gridPropertyDetails.IsPath == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.MovementPenalty = pathMovementPenalty;
                            }
                            else
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.MovementPenalty = defaultMovementPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
        int dstY = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);

        if (dstX > dstY)
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }

    private Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition)
    {
        if (neighbourNodeXPosition >= gridWidth || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= gridHeight ||
            neighbourNodeYPosition < 0)
        {
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);
        if (neighbourNode.IsObstacle || closedNodeList.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }

    private void EvaluateCurrentNodeNeighbours(Node currentNode)
    {
        Vector2Int currentNodeGridPosition = currentNode.GridPosition;

        Node validNeighbourNode;

        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                if(i == 0 && j == 0)
                    continue;

                validNeighbourNode =
                    GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j);

                if (validNeighbourNode != null)
                {
                    int newCostToNeighbour;

                    if (observeMovementPenalties)
                    {
                        newCostToNeighbour = currentNode.GCost + GetDistance(currentNode, validNeighbourNode) +
                                             validNeighbourNode.MovementPenalty;
                    }
                    else
                    {
                        newCostToNeighbour = currentNode.GCost + GetDistance(currentNode, validNeighbourNode);
                    }

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.GCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.GCost = newCostToNeighbour;
                        validNeighbourNode.HCost = GetDistance(validNeighbourNode, targetNode);

                        validNeighbourNode.ParentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private bool FindShortestPath()
    {
        openNodeList.Add(startNode);

        while (openNodeList.Count > 0)
        {
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            closedNodeList.Add(currentNode);

            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            EvaluateCurrentNodeNeighbours(currentNode);
        }

        return pathFound;
    }

    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = targetNode;

        while (nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            npcMovementStep.SceneName = sceneName;
            npcMovementStep.GridCoordinate =
                new Vector2Int(nextNode.GridPosition.x + originX, nextNode.GridPosition.y + originY);
            npcMovementStepStack.Push(npcMovementStep);
            nextNode = nextNode.ParentNode;
        }
    }

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition,
        Stack<NPCMovementStep> npcMovementStepStack)
    {
        pathFound = false;
        if (PopulateGridNodesFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
        {
            if (FindShortestPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);
                return true;
            }
        }
        return false;
    }
}
