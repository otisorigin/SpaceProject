using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Game.Scripts.Utils.Collections;
using ModestTree;
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
        if (!_map.UnitCanEnterTile((int)x,(int)y))
        {
            return;
        }
        var unit = _unitManager.GetSelectedUnitMovementSystem();
        var source = _graph[unit.tileX, unit.tileY];
        var target = GetAvailableTarget(x, y, _unitManager.SelectedUnit);
        PathFindingUtils.GeneratePathTo(source, target, _dynamicObstacleNodes, _map, _unitManager);
    }

    private Node GetAvailableTarget(float x, float y, Unit unit)
    {
        var availableNodes = unit.GetComponentInChildren<MovementSystem>().availableNodesToMove;
        var sizeX = _map.mapSizeX;
        var sizeY = _map.mapSizeY;
        float xf = x + ShiftFactor;
        float yf = y + ShiftFactor;

        if (!availableNodes.Contains(_graph[xf, yf]))
        {
            return GetEmptyNode();
        }

        if (y.Equals(sizeY))
        {
            return _graph[xf, yf - 1];
        }
        if (x.Equals(sizeX) && x.Equals(sizeY))
        {
            return _graph[xf - 1, yf - 1];
        }
        var isTopRightCorner = x < sizeX && y < sizeY && !availableNodes.Contains(_graph[xf + 1, yf]) &&
                               !availableNodes.Contains(_graph[xf, yf + 1]);
        if (isTopRightCorner)
        {
            return _graph[xf-1, yf-1];
        }
        var isTopLeftCorner = x > 0 && y < sizeY && !availableNodes.Contains(_graph[xf, yf + 1]) &&
                              !availableNodes.Contains(_graph[xf - 1, yf]);
        if (isTopLeftCorner)
        {
            return _graph[xf, yf-1];
        }
        var isTopBorder = !availableNodes.Contains(_graph[xf, yf + 1]);
        if (isTopBorder)
        {
            return _graph[xf, yf-1];
        }
        var isRightBorder = !availableNodes.Contains(_graph[xf + 1, yf]);
        if (isRightBorder)
        {
            return _graph[xf-1, yf];
        }
        if (_graph.ContainsKey(xf, yf) && availableNodes.Contains(_graph[xf, yf]))
        {
            return _graph[xf, yf];
        }

        return GetEmptyNode();
    }

    private static Node GetEmptyNode()
    {
        return new Node {x = -1, y = -1, neighbours = new List<Node>()};
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

    public HashSet<Node> GetAvailableNodes()
    {
        //PathFindingUtils.GetAvailableNodes(_dynamicObstacleNodes, _graph.Values, _unitManager, _map);
         var unit = _unitManager.GetSelectedUnitMovementSystem();
        
        if (unit.RemainingMovement == 0)
        {
            return new HashSet<Node>();
        }
        
        var unvisited = new List<Node>();
        
        var dist = new Dictionary<Node, float>();
        var prev = new Dictionary<Node, Node>();
        
        Node source = _graph[unit.tileX, unit.tileY];
        
        dist[source] = 0;
        prev[source] = null;
        
        foreach (var node in _graph.Values)
        {
            if (node != source)
            {
                dist[node] = Mathf.Infinity;
                prev[node] = null;
            }
        
            if (!_dynamicObstacleNodes.Contains(node) &&
                !IsRemainingMovementEnoughWithCost(source, node, unit.RemainingMovement))
            {
                unvisited.Add(node);
            }
        }
        
        var availableNodes = new HashSet<Node>();
        
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
        
            unvisited.Remove(u);
            availableNodes.Add(u);
        
            foreach (var v in u.neighbours)
            {
                if (!_dynamicObstacleNodes.Contains(v))
                {
                    float alt = dist[u] + _map.CostToEnterTile(u, v);
                    if (unit.RemainingMovement + 2 < alt && !float.IsPositiveInfinity(_map.CostToEnterTile(u, v)))
                    {
                        availableNodes.Remove(u);
                    }
        
                    if (alt < dist[v] && alt <= unit.RemainingMovement)
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }
        }
        
        // // map.ShowAvailableTilesToMove(availableNodes);
        return AddNodesForUnit2x2(availableNodes);
    }

    private HashSet<Node> AddNodesForUnit2x2(HashSet<Node> nodes)
    {
        var newAvailableNodes = new HashSet<Node>();
        foreach (var node in nodes)
        {
            if (_map.UnitCanEnterNode(node.x, node.y))
            {
                newAvailableNodes.Add(node);
                var newY = node.y + 1;
                if (newY < _map.mapSizeY)
                {
                    newAvailableNodes.Add(_graph[node.x, newY]);
                }
                var newX = node.x + 1;
                if (newX < _map.mapSizeX)
                {
                    newAvailableNodes.Add(_graph[newX, node.y]);
                }

                if (newX < _map.mapSizeX && newY < _map.mapSizeY)
                {
                    newAvailableNodes.Add(_graph[node.x+1, node.y+1]);
                } 
            }
        }

        return newAvailableNodes;
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