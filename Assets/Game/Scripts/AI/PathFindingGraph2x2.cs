using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Zenject;
using static PathFindingUtils;

public class PathFindingGraph2x2 : MonoBehaviour, IPathFindingGraph
{
    [Inject] private TileMap _map;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;

    private static readonly float ShiftFactor = 0.5f;
    private static int UnitSize = 2;

    private List<Node> _dynamicObstacleNodes;

    private DoubleKeyDictionary<float, float, Node> _graph;
    //Node[,] _graph;

    //TODO Как определять куда генерировать путь?
    public void GeneratePathTo(float x, float y)
    {
         if (!_map.UnitCanEnterTile((int)x, (int)y))
        {
            return;
        }

         float xf = x + ShiftFactor;
         float yf = y + ShiftFactor;
        
        var unit = _unitManager.SelectedUnit.GetComponent<Unit>();
        unit.CurrentPath = null;

        var unvisited = new List<Node>();

        var dist = new Dictionary<Node, float>();
        var prev = new Dictionary<Node, Node>();

        Node source = _graph[unit.tileX, unit.tileY];
        Node target = _graph[xf, yf];

        dist[source] = 0;
        prev[source] = null;

        foreach (var node in _graph.Values)
        {
            if (node != source)
            {
                dist[node] = Mathf.Infinity;
                prev[node] = null;
            }

            if (!_dynamicObstacleNodes.Contains(node))
            {
                unvisited.Add(node);
            }
        }

        while (unvisited.Count > 0)
        {
            //unvisited node with smallest distance
            Node u = null;

            foreach (var possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
                break;
            }

            unvisited.Remove(u);

            foreach (var v in u.neighbours)
            {
                if (!_dynamicObstacleNodes.Contains(v) /*&& !_diagonalNeighbourObstacleNodes.Contains(v)*/)
                {
                    float alt = dist[u] + _map.CostToEnterTile(u, v);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    } 
                }
                
            }
        }

        //we found shortest way to our target or there is no way to our target
        if (prev[target] == null)
        {
            //no way between our target and the source
            return;
        }

        var currentPath = new List<Node>();

        Node curr = target;

        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }

        currentPath.Reverse();
        unit.CurrentPath = currentPath;
    }

    public void SetDynamicObstacleNodes()
    {
        Debug.Log("Set Dynamic Obstacle Nodes for PathfindingGrapgh2x2 not implemented!!!");
        _dynamicObstacleNodes = new List<Node>();
    }

    public List<Node> GetDynamicObstacleNodes()
    {
        return _dynamicObstacleNodes;
    }

    public void GeneratePathFindingGraph()
    {
        //Initialize collection with Nodes
        _graph = new DoubleKeyDictionary<float, float, Node>();
        //_graph = new Dictionary<float, Dictionary<float, Node>>();

        //Initialize a Node for each spot in the array
        for (float x = 0 + ShiftFactor; x < _map.mapSizeX; x++)
        {
            for (float y = 0 + ShiftFactor; y < _map.mapSizeY; y++)
            {
                _graph[x, y] = new Node {x = x, y = y};
            }
        }

        for (float x = 0 + ShiftFactor; x < _map.mapSizeX-1; x++)
        {
            for (float y = 0 + ShiftFactor; y < _map.mapSizeY-1; y++)
            {
                if (x > 1 && _map.UnitCanEnterNode(x-1, y))
                {
                    _graph[x, y].neighbours.Add(_graph[x - 1, y]);
                        if (y > 1 && 
                            _map.UnitCanEnterNode(x, y - 1) &&
                            _map.UnitCanEnterNode(x - 1, y) &&
                            _map.UnitCanEnterNode(x - 1, y - 1))
                                _graph[x, y].neighbours.Add(_graph[x - 1, y - 1]);
                        if (y < _map.mapSizeY - 2 &&
                            _map.UnitCanEnterNode(x, y + 1) &&
                            _map.UnitCanEnterNode(x - 1, y) &&
                            _map.UnitCanEnterNode(x - 1, y + 1))
                                _graph[x, y].neighbours.Add(_graph[x - 1, y + 1]);
                }

                //Right
                if (x < _map.mapSizeX - 2 && _map.UnitCanEnterNode(x + 1, y))
                {
                    _graph[x, y].neighbours.Add(_graph[x + 1, y]);
                        if (y > 1 && 
                            _map.UnitCanEnterNode(x, y - 1) &&
                            _map.UnitCanEnterNode(x + 1, y) &&
                            _map.UnitCanEnterNode(x + 1, y - 1))
                                _graph[x, y].neighbours.Add(_graph[x + 1, y - 1]);
                        if (y < _map.mapSizeX - 2 && 
                            _map.UnitCanEnterNode(x, y + 1) &&
                            _map.UnitCanEnterNode(x + 1, y) &&
                            _map.UnitCanEnterNode(x + 1, y + 1))
                                _graph[x, y].neighbours.Add(_graph[x + 1, y + 1]);
                }

                //Up and down
                if (y > 1 && _map.UnitCanEnterNode(x, y-1))
                {
                    _graph[x, y].neighbours.Add(_graph[x, y - 1]);
                }

                if (y < _map.mapSizeX - 2 && _map.UnitCanEnterNode(x, y+1))
                {
                    _graph[x, y].neighbours.Add(_graph[x, y + 1]);
                }
            }
        }
    }
    
//    void OnDrawGizmos()
//    {
//        Gizmos.color = Color.yellow;
//        _graph.Values.ToList().ForEach(node =>
//        {
//            Gizmos.DrawSphere(new Vector3(node.x, node.y, -1), 0.2f);
//        });
//    }
//
//    private void Draw(List<Node> nodes)
//    {
//        nodes.ToList().ForEach(neighbour =>
//        {
//            Gizmos.DrawSphere(new Vector3(neighbour.x, neighbour.y, -1), 0.2f);
//        });
//    }
}