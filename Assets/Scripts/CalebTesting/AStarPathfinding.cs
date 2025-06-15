using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class AStarPathfinding : MonoBehaviour
{
    public Transform seeker;
    public Transform target;

    AreaGrid grid;

    NodeHeap<Node> openSet;


    private void Awake()
    {
        grid = GetComponent<AreaGrid>();
    }

    private void Start()
    {
        openSet = new NodeHeap<Node>(grid.MaxSize);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath(seeker.position, target.position);
        }
    }

    private void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();


        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        openSet.Clear();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0) // things are only added to the openSet when they are up for eval
        {
            Node currentNode = openSet.RemoveFirst();
            //NodeHeap<Node> openSet = new NodeHeap<Node>(grid.MaxSize);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) // PATH FOUND
            {
                sw.Stop();
                print("Path found " + sw.ElapsedMilliseconds);
                RetracePath(startNode, targetNode);
                return;
            }

            //Debug.Log(grid.GetNeighbors(currentNode).Count);

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) // if path to neighbor is shorter or they not in the open path (which means also not in closed)
                {
                    neighbor.gCost = newMovementCostToNeighbor; // each neighbor towards the thing should be all previous gCost added?
                    neighbor.hCost = GetDistance(neighbor, targetNode); // from neighbor to target
                    neighbor.parent = currentNode; // sets last node as parent, making chain

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor); // adds to open set if not in it
                    }
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode; // trace backwards, since all nodes in the path have the previous as a parent

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent; // cycles through
        }

        path.Reverse(); // since traced backwards

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        int smallest;
        int largest;
        int secondLargest;

        largest = Mathf.Max(dstX, dstY, dstZ);
        smallest = Mathf.Min(dstX, dstY, dstZ);
        if (dstX != largest && dstX != smallest)
        {
            secondLargest = dstX;
        }
        else if (dstY != largest && dstY != smallest)
        {
            secondLargest= dstY;
        }
        else
        {
            secondLargest= dstZ;
        }

        return smallest * 17 + (secondLargest - smallest) * 14 + (largest - secondLargest) * 10;
    }
}
