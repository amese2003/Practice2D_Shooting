using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject back_tile;
    public Grid tile_grid { get; private set; }

    public LayerMask unwalkable;
    Node[,] grid_map;

    public int col_Grid;
    public int row_Grid;

    float nodeDiameter;
    float nodeRadius;

    int gridTotalSizeX, gridTotalSizeY;

    public bool onlyDisplayPathGizmos;

    public List<Node> path;

    private void Start()
    {
        if (!tile_grid) tile_grid = back_tile.GetComponent<Grid>();

        nodeDiameter = tile_grid.cellSize.x;
        nodeRadius = tile_grid.cellSize.x / 2;

        gridTotalSizeX = Mathf.RoundToInt(row_Grid / nodeDiameter);
        gridTotalSizeY = Mathf.RoundToInt(col_Grid / nodeDiameter);

        CreateNode();

    }

    private void Update()
    {
        Trace();
    }


    private void CreateNode()
    {       
        grid_map = new Node[col_Grid, row_Grid];
        Vector3 worldBottomLeft = transform.position - Vector3.right * row_Grid / 2 - Vector3.up * col_Grid / 2;        

        for (int col = 0; col < col_Grid; col++)
        {
            for(int row = 0; row < row_Grid; row++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (row * nodeDiameter + nodeRadius) + Vector3.up * (col * nodeDiameter + nodeRadius);
                //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkable));

                bool walkable = true;                
                Collider2D temp = Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkable);
                if (temp != null)
                    walkable = false;

                grid_map[col, row] = new Node(tile_grid.WorldToCell(worldPoint), worldPoint, walkable, col, row);
            }
        }
    }

    async void Trace()
    {
        FindPath(GameManager.Instance.enemyPos, GameManager.Instance.playerPos);
    }

    //async void temp()
    //{
    //    List<Node> te;
    //    await Task.Run(() => te = FindPath(Vector3.zero, Vector3.one));
    //    await Task.Delay(1000);
    //}

    private List<Node> FindPath(Vector3 startPos, Vector3 endPos)
    {
        Vector3 cellPos = tile_grid.WorldToCell(startPos);

        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(endPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);               

            List<Node> neighbor = getNeighbor(currentNode);

            foreach(Node n_node in neighbor)
            {
                if (!n_node.walkable || closedSet.Contains(n_node))
                    continue;

                int newMoveCost = currentNode.gCost + GetDistance(currentNode, n_node);

                if(newMoveCost < n_node.gCost || !openSet.Contains(n_node))
                {
                    n_node.gCost = newMoveCost;
                    n_node.hCost = GetDistance(n_node, targetNode);
                    n_node.parent = currentNode;

                    if (!openSet.Contains(n_node))
                        openSet.Add(n_node);
                }
            }
        }

        return null;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> getPath = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            getPath.Add(currentNode);
            currentNode = currentNode.parent;
        }

        getPath.Reverse();
        path = getPath;
        return getPath;
    }

    public List<Node> getNeighbor(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int col = -1; col <= 1; col++)
        {
            for (int row = -1; row <= 1; row++)
            {
                if (col == 0 && row == 0)
                    continue;

                int checkRow = node.row + row;
                int checkCol = node.col + col;

                if (checkRow >= 0 && checkRow < row_Grid && checkCol >= 0 && checkCol < col_Grid)
                {
                    neighbours.Add(grid_map[checkCol, checkRow]);
                }
            }
        }        

        return neighbours;
    }

    int GetDistance(Node seeker, Node target)
    {      
        int distanceX = Mathf.Abs((int)seeker.cellPos.x - (int)target.cellPos.x);
        int distanceY = Mathf.Abs((int)seeker.cellPos.y - (int)target.cellPos.y);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);

        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    private Node NodeFromWorldPoint(Vector3 pos) {

        float percentX = (pos.x + row_Grid / 2) / row_Grid;
        float percentY = (pos.y + col_Grid / 2) / col_Grid;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int row = Mathf.RoundToInt((gridTotalSizeX - 1) * percentX);
        int col = Mathf.RoundToInt((gridTotalSizeY - 1) * percentY);

        Debug.Log(grid_map[col, row].worldPos);
        return grid_map[col, row];        
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1f, 0));

        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPos, Vector3.one);
                }
            }

        }
        else
        {

            if (grid_map != null)
            {
                //Node playerNode = NodeFromWorldPoint(player.position);

                foreach (Node n in grid_map)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;

                    if (path != null)
                        if (path.Contains(n))
                            Gizmos.color = Color.yellow;


                    Gizmos.DrawCube(n.worldPos, Vector3.one);
                }
            }
        }
    }
}
