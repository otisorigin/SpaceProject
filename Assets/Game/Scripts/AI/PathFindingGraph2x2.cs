using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PathFindingGraph2x2 : MonoBehaviour, IPathFindingGraph
{
    [Inject] private TileMap _map;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;

    private List<Node> _dynamicObstacleNodes;
    Node[,] _graph;
    
    public void GeneratePathTo(int x, int y)
    {
        Debug.Log("Generate Path to for PathfindingGrapgh2x2 not implemented!!!");
    }

    public void SetDynamicObstacleNodes()
    {
        Debug.Log("Set Dynamic Obstacle Nodes for PathfindingGrapgh2x2 not implemented!!!");
    }

    public List<Node> GetDynamicObstacleNodes()
    {
        return _dynamicObstacleNodes;
    }

    public void GeneratePathfindingGrapgh()
    {
        Debug.Log("Generate PathfindingGrapgh2x2 not implemented!!!");
    }
}