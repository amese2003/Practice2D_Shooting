using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    GridManager grid;
    private void Awake()
    {
        grid = GetComponent<GridManager>();
    }


    public void FindPath(PathRequest _request, Action<PathResult> _callback)
    {
        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(_request.pathStart);
        Node targetNode = grid.NodeFromWorldPoint(_request.pathEnd);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                List<Node> neighbors = grid.getNeighbor(currentNode);

                foreach (Node neighbor in neighbors)
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                        continue;

                    int newMoveCost = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movePenalty;

                    if (newMoveCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMoveCost;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                        else
                            openSet.UpdateItem(neighbor);
                    }
                }
            }
        }

        if (pathSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode);
            pathSuccess = wayPoints.Length > 0;
        }

        _callback(new PathResult(wayPoints, pathSuccess, _request.callback));
    }

    Vector3[] RetracePath(Node _start, Node _end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = _end;

        while(currentNode != _start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] wayPoints = SimplifyPath(path);
        Array.Reverse(wayPoints);
        return wayPoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].row - path[i].row, path[i - 1].col - path[i].col);
            if (directionNew != directionOld)
                wayPoints.Add(path[i].worldPos);

            directionOld = directionNew;
        }
        return wayPoints.ToArray();
    }


    int GetDistance(Node seeker, Node target)
    {
        int distanceX = Mathf.Abs((int)seeker.cellPos.x - (int)target.cellPos.x);
        int distanceY = Mathf.Abs((int)seeker.cellPos.y - (int)target.cellPos.y);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);

        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
