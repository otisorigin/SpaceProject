using System.Collections.Generic;
using UnityEngine;

public class PathFindingGraph3x3 : MonoBehaviour, IPathFindingGraph
{
    private void Start()
    {
        GeneratePathfindingGrapgh();
    }
    
    public void GeneratePathTo(int x, int y)
    {
        Debug.Log("Generate Path To for PathfindingGrapgh2x2 not implemented!!!");
    }

    public void SetDynamicObstacleNodes()
    {
        Debug.Log("Set Dynamic Obstacle Nodes for PathfindingGrapgh2x2 not implemented!!!");
    }

    public List<Node> GetDynamicObstacleNodes()
    {
        throw new System.NotImplementedException();
    }

    public void GeneratePathfindingGrapgh()
    {
        Debug.Log("Generate PathfindingGrapgh2x2 not implemented!!!");
    }
}