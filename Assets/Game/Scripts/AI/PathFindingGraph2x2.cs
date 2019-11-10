using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Zenject;

public class PathFindingGraph2x2 : MonoBehaviour, IPathFindingGraph
{
    [Inject] private TileMap _map;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;

    private static readonly float ShiftFactor = 0.5f;
    private static int UnitSize = 2;

    private List<Node> _dynamicObstacleNodes;

    private Dictionary<Tuple<float, float>,Node> _graph;
    //Node[,] _graph;

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

    public void GeneratePathFindingGraph()
    {
        //Initialize collection with Nodes
        _graph = new Dictionary<Tuple<float, float>, Node>();
        //_graph = new Dictionary<float, Dictionary<float, Node>>();

        //Initialize a Node for each spot in the array
        for (int x = 0; x < _map.mapSizeX - 1; x++)
        {
            for (int y = 0; y < _map.mapSizeY - 1; y++)
            {
                _graph[new Tuple<float, float>(x, y)] = new Node {x = x + ShiftFactor, y = y + ShiftFactor};
            }
        }
        
    }
}