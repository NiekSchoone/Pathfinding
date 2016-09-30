using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    Grid nodeGrid;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        nodeGrid = GetComponent<Grid>();
    }

    /// <summary>
    /// Starts a Coroutine which will find a path from start position to end position.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="targetPos"></param>
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    /// <summary>
    /// Coroutine which is started in the "StartFindPath" method.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathFound = false;

        Node startNode = nodeGrid.GetNodeFromWorldPoint(startPosition);
        Node targetNode = nodeGrid.GetNodeFromWorldPoint(targetPosition);

        if(!targetNode.walkable) {
            List<Node> targetNeigbours = nodeGrid.GetNeighbours(targetNode);
            for (int i = 0; i < targetNeigbours.Count; i++) {
                if(targetNeigbours[i].walkable) {
                    targetNode = targetNeigbours[i];
                    break;
                }
            }
        }

        if (targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(nodeGrid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathFound = true;
                    break;
                }

                foreach (Node neighbour in nodeGrid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    int newMovementCostToNeightbour = currentNode.gCost + GetDistanceToNode(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeightbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeightbour;
                        neighbour.hCost = GetDistanceToNode(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathFound)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathFound);
            
    }

    int GetDistanceToNode(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }else
        {
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }

    Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();

        Node checkPoint = path[0];
        waypoints.Add(checkPoint.worldPostion);

        for (int i = 1; i < path.Count; i++) {
            Node currentPoint = path[i];
            Node validPoint = path[i - 1];
            if(!WalkableLine(checkPoint.worldPostion, currentPoint.worldPostion)) {
                checkPoint = validPoint;
                waypoints.Add(checkPoint.worldPostion);
            }
        }
        return waypoints.ToArray();
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    bool WalkableLine(Vector3 pointA, Vector3 pointB) {
        Vector3 dir = (pointB - pointA).normalized;
        float distance = Vector3.Distance(pointA, pointB);

        RaycastHit hitInfo;
        Ray ray = new Ray(pointA, dir);

        if(Physics.Raycast(ray, out hitInfo, distance)) {
            if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Unwalkable")) {
                return false;
            }
        }
        return true;
    }
}
