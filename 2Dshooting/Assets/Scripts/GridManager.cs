using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject back_tile;
    public Grid tile_grid { get; private set; }

    public LayerMask unwalkableMask;
    Node[,] grid;

    public TerrainType[] walkableRegions;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    int obstacleProximityPenalty = 10;

    public Vector2 gridWorldSize;

    float nodeDiameter;
    float nodeRadius;

    int gridTotalSizeX, gridTotalSizeY;
    LayerMask walkableMask;

    public bool onlyDisplayPathGizmos;

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;

    public int MaxSize
    {
        get
        {
            return gridTotalSizeX * gridTotalSizeY;
        }
    }

    private void Awake()
    {
        if (!tile_grid) tile_grid = back_tile.GetComponent<Grid>();

        nodeDiameter = nodeRadius * 2; // grid간의 간격 (중앙에서 중앙으로)
        gridTotalSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridTotalSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value += region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
        CreateNode();
    }

    private void CreateNode()
    {
        grid = new Node[gridTotalSizeX, gridTotalSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridTotalSizeX / 2 - Vector3.up * gridTotalSizeY / 2;        

        for (int row = 0; row < gridTotalSizeX; row++)
        {
            for(int col = 0; col < gridTotalSizeY; col++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (row * nodeDiameter + nodeRadius) + Vector3.up * (col * nodeDiameter + nodeRadius);
                //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));    
                Collider2D temp = Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask);
                int movementPanelty = 0;
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                if (walkable)
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, walkableMask))
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPanelty);
                }

                if (!walkable)
                {
                    movementPanelty += obstacleProximityPenalty;
                }

                grid[row, col] = new Node(tile_grid.WorldToCell(worldPoint), worldPoint, walkable, row, col, movementPanelty);
            }
        }
    }

    private void BlurPenaltyMap(int blurSize)
    {
        int kernalSize = blurSize * 2 + 1;
        int kernalExtends = (kernalSize - 1) / 2;

        int[,] penaltyHorizontalPass = new int[gridTotalSizeX, gridTotalSizeY];
        int[,] penaltyVeritcalPass = new int[gridTotalSizeX, gridTotalSizeY];

        for(int col = 0; col < gridTotalSizeY; col++)
        {
            for(int row = -kernalExtends; row <= kernalExtends; row++)
            {
                int sampleX = Mathf.Clamp(row, 0, kernalExtends);
                penaltyHorizontalPass[0, col] += grid[sampleX, col].movePenalty;
            }

            for(int row = 1; row < gridTotalSizeX; row++)
            {
                int removeIDx = Mathf.Clamp(row - kernalExtends - 1, 0, gridTotalSizeX);
                int addIndex = Mathf.Clamp(row + kernalExtends, 0, gridTotalSizeX - 1);

                penaltyHorizontalPass[row, col] = penaltyHorizontalPass[row - 1, col] - grid[removeIDx, col].movePenalty + grid[addIndex, col].movePenalty;
            }
        }

        for(int row = 0; row < gridTotalSizeX; row++)
        {
            for(int col = -kernalExtends; col<=kernalExtends; col++)
            {
                int sampleY = Mathf.Clamp(row, 0, kernalExtends);
                penaltyVeritcalPass[row, 0] += penaltyHorizontalPass[row, sampleY];
            }

            for(int col = 1; col <gridTotalSizeY; col++)
            {
                int removeIDx = Mathf.Clamp(col - kernalExtends - 1, 0, gridTotalSizeY);
                int addIndex = Mathf.Clamp(col + kernalExtends, 0, gridTotalSizeY - 1);

                penaltyVeritcalPass[row, col] = penaltyVeritcalPass[row, col - 1] - penaltyHorizontalPass[row, removeIDx] + penaltyHorizontalPass[row, addIndex];
                int blurredPenalty = Mathf.RoundToInt((float)penaltyVeritcalPass[row, col] / (kernalSize * kernalSize));
                grid[row, col].movePenalty = blurredPenalty;

                if (blurredPenalty > penaltyMax)
                    penaltyMax = blurredPenalty;

                if (blurredPenalty < penaltyMin)
                    penaltyMin = blurredPenalty;
            }
        }
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

                if (checkRow >= 0 && checkRow < gridTotalSizeX && checkCol >= 0 && checkCol < gridTotalSizeY)
                {
                    neighbours.Add(grid[checkCol, checkRow]);
                }
            }
        }        

        return neighbours;
    }

    

    public Node NodeFromWorldPoint(Vector3 pos) {

        float percentX = (pos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (pos.y + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int row = Mathf.RoundToInt((gridTotalSizeX - 1) * percentX);
        int col = Mathf.RoundToInt((gridTotalSizeY - 1) * percentY);
        return grid[col, row];        
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && onlyDisplayPathGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movePenalty));

                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                Gizmos.DrawCube(n.worldPos, Vector3.one * nodeDiameter);
            }
        }
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
