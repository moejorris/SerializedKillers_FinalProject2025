using UnityEngine;
using System.Collections.Generic;

public class AStarTest : MonoBehaviour
{
    //public Transform player;
    public LayerMask unwalkableMask;
    public Vector3 gridWorldSize;
    public float nodeRadius;
    Node[,,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY, gridSizeZ;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateGrid()
    {
        Debug.Log("The amount should be: " + gridSizeX * gridSizeY * gridSizeZ);
        int amount = 0;
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2 - Vector3.up * gridWorldSize.z / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            //Debug.Log("The " + x + "pass of X");
            for (int y = 0; y < gridSizeY; y++)
            {
                //Debug.Log("The " + y + "pass of Y");
                for (int z = 0; z < gridSizeZ; z++)
                {
                    //Debug.Log("The " + z + "pass of Z");
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius) + Vector3.up * (z * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                    grid[x,y,z] = new Node(walkable, worldPoint, x,y,z);
                    amount++;
                }
            }
        }

        Debug.Log("DOne with pass. Amount is: " + amount);
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        //float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        //float percentZ = (worldPosition.y + gridWorldSize.z / 2) / gridWorldSize.z;

        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y; // zz,yy swap
        float percentZ = (worldPosition.y - transform.position.y + gridWorldSize.z / 2) / gridWorldSize.z; // yy,zz swap

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        return grid[x, y, z];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        if (grid != null)
        {
            //Node playerNode = NodeFromWorldPoint(player.position);
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.clear : Color.red;
                //if (playerNode == n)
                //{
                //    Gizmos.color = Color.cyan;
                //}
                Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
