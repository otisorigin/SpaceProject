using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Game.Scripts.Utils.Collections;
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

    public void GeneratePathTo(float x, float y)
    {
        if (!_map.UnitCanEnterTile((int) x, (int) y))
        {
            return;
        }

        float xf = x + ShiftFactor;
        float yf = y + ShiftFactor;

        var unit = _unitManager.GetSelectedUnitMovementSystem();
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
        _dynamicObstacleNodes = new List<Node>();
        foreach (var uObject in _unitManager.GetUnitObjects())
        {
            var unit = uObject.GetComponent<Unit>().GetComponentInChildren<MovementSystem>();
            var unitScale = uObject.GetComponent<Unit>().GetScale();
            if (unitScale == 1)
            {
                SetUnitObstacle1X1(unit.tileX, unit.tileY);
            }

            if (unitScale == 2)
            {
                SetUnitObstacle2X2(unit.tileX, unit.tileY);
            }

            if (unitScale == 3)
            {
                SetUnitObstacle3X3(unit.tileX, unit.tileY);
            }
        }
    }

    public List<Node> GetAvailableNodes()
    {
        Debug.Log("GetAvailableNodes not implemented for PathFindingGrapgh3x3");
        return null;
    }

    private void SetUnitObstacle1X1(float tileX, float tileY)
    {
        if (tileX + 1 <= _map.mapSizeX)
        {
            if (tileY + 1 <= _map.mapSizeY)
            {
                _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor, tileY + ShiftFactor]);
            }

            if (tileY > 0)
            {
                _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor, tileY - ShiftFactor]);
            }
        }

        if (tileX > 0)
        {
            if (tileY + 1 <= _map.mapSizeY)
            {
                _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor, tileY + ShiftFactor]);
            }

            if (tileY > 0)
            {
                _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor, tileY - ShiftFactor]);
            }
        }
    }

    private void SetUnitObstacle2X2(float tileX, float tileY)
    {
        _dynamicObstacleNodes.Add(_graph[tileX, tileY]);
        if (tileY <= _map.mapSizeY - ShiftFactor)
        {
            _dynamicObstacleNodes.Add(_graph[tileX, tileY + 1]);
        }

        if (tileY > 0)
        {
            _dynamicObstacleNodes.Add(_graph[tileX, tileY - 1]);
        }

        if (tileX <= _map.mapSizeX - ShiftFactor)
        {
            _dynamicObstacleNodes.Add(_graph[tileX + 1, tileY]);
            if (tileY <= _map.mapSizeY - ShiftFactor)
            {
                _dynamicObstacleNodes.Add(_graph[tileX + 1, tileY + 1]);
            }

            if (tileY > 0)
            {
                _dynamicObstacleNodes.Add(_graph[tileX + 1, tileY - 1]);
            }
        }

        if (tileX > ShiftFactor)
        {
            _dynamicObstacleNodes.Add(_graph[tileX - 1, tileY]);
            if (tileY <= _map.mapSizeY - ShiftFactor)
            {
                _dynamicObstacleNodes.Add(_graph[tileX - 1, tileY + 1]);
            }

            if (tileY > ShiftFactor)
            {
                _dynamicObstacleNodes.Add(_graph[tileX - 1, tileY - 1]);
            }
        }
    }

    private void SetUnitObstacle3X3(float tileX, float tileY)
    {
        _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor, tileY + ShiftFactor]);
        _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor, tileY - ShiftFactor]);
        _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor, tileY + ShiftFactor]);
        _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor, tileY - ShiftFactor]);
        if (_map.mapSizeY - 2 > tileY)
        {
            _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor, tileY + ShiftFactor + 1]);
            _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor, tileY + ShiftFactor + 1]);
        }

        if (tileY > 1)
        {
            _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor, tileY - ShiftFactor - 1]);
            _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor, tileY - ShiftFactor - 1]);
        }

        if (_map.mapSizeX - 2 > tileX)
        {
            _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor + 1, tileY + ShiftFactor]);
            _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor + 1, tileY - ShiftFactor]);
            if (_map.mapSizeY - 2 > tileY)
            {
                _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor + 1, tileY + ShiftFactor + 1]);
            }

            if (tileY > 1)
            {
                _dynamicObstacleNodes.Add(_graph[tileX + ShiftFactor + 1, tileY - ShiftFactor - 1]);
            }
        }

        if (tileX > 1)
        {
            _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor - 1, tileY + ShiftFactor]);
            _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor - 1, tileY - ShiftFactor]);
            if (_map.mapSizeY - 2 > tileY)
            {
                _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor - 1, tileY + ShiftFactor + 1]);
            }

            if (tileY > 1)
            {
                _dynamicObstacleNodes.Add(_graph[tileX - ShiftFactor - 1, tileY - ShiftFactor - 1]);
            }
        }
    }

    public List<Node> GetDynamicObstacleNodes()
    {
        return _dynamicObstacleNodes;
    }

    public void GeneratePathFindingGraph()
    {
        //Initialize collection with Nodes
        _graph = new DoubleKeyDictionary<float, float, Node>();
        //Initialize a Node for each spot in the array
        for (float x = 0 + ShiftFactor; x < _map.mapSizeX; x++)
        {
            for (float y = 0 + ShiftFactor; y < _map.mapSizeY; y++)
            {
                _graph[x, y] = new Node {x = x, y = y};
            }
        }

        for (float x = 0 + ShiftFactor; x < _map.mapSizeX - 1; x++)
        {
            for (float y = 0 + ShiftFactor; y < _map.mapSizeY - 1; y++)
            {
                if (x > 1 && _map.UnitCanEnterNode(x - 1, y))
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
                if (y > 1 && _map.UnitCanEnterNode(x, y - 1))
                {
                    _graph[x, y].neighbours.Add(_graph[x, y - 1]);
                }

                if (y < _map.mapSizeX - 2 && _map.UnitCanEnterNode(x, y + 1))
                {
                    _graph[x, y].neighbours.Add(_graph[x, y + 1]);
                }
            }
        }
    }
}