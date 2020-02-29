using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PathFindingGraph3x3 : MonoBehaviour, IPathFindingGraph
{
    [Inject] private TileMap _map;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;

    private static int UnitSize = 3;

    private List<Node> _dynamicObstacleNodes;
    Node[,] _graph;

    public void GeneratePathTo(float x, float y)
    {
        if (!_map.UnitCanEnterTile((int) x, (int) y))
        {
            return;
        }
        var unit = _unitManager.GetSelectedUnitMovementSystem();
        var source = _graph[(int) unit.tileX, (int) unit.tileY];
        var target = GetAvailableTarget(x, y, _unitManager.SelectedUnit);//_graph[(int) x, (int) y];
        PathFindingUtils.GeneratePathTo(source, target, _dynamicObstacleNodes, _map, _unitManager);
    }
    
    private Node GetAvailableTarget(float x, float y, Unit unit)
    {
        var availableNodes = unit.GetComponentInChildren<MovementSystem>().availableNodesToMove;
        var sizeX = _map.mapSizeX;
        var sizeY = _map.mapSizeY;
        if (x.Equals(0) && y.Equals(0))
        {
            return _graph[(int) x+1, (int) y+1];
        }
        if (x.Equals(sizeX) && x.Equals(sizeY))
        {
            return _graph[(int) x-1, (int) y-1];
        }
        if (x.Equals(0))
        {
            return _graph[(int) x+1, (int) y];
        }
        if (y.Equals(0))
        {
            return _graph[(int) x, (int) y+1];
        }
        if (x.Equals(sizeX))
        {
            return _graph[(int) x-1, (int) y];
        }
        if (y.Equals(sizeY))
        {
            return _graph[(int) x, (int) y-1];
        }
        var isTopRightCorner = !availableNodes.Contains(_graph[(int) x + 1, (int) y]) &&
                                !availableNodes.Contains(_graph[(int) x, (int) y + 1]);
        if (isTopRightCorner)
        {
            return _graph[(int) x-1, (int) y-1]; 
        }
        var isBottomLeftCorner = !availableNodes.Contains(_graph[(int) x - 1, (int) y]) &&
                                 !availableNodes.Contains(_graph[(int) x, (int) y - 1]);
        if (isBottomLeftCorner)
        {
            return _graph[(int) x+1, (int) y+1]; 
        }
        var isTopLeftCorner = !availableNodes.Contains(_graph[(int) x, (int) y + 1]) &&
                              !availableNodes.Contains(_graph[(int) x - 1, (int) y]);
        if (isTopLeftCorner)
        {
            return _graph[(int) x+1, (int) y-1]; 
        }
        var isBottomRightCorner = !availableNodes.Contains(_graph[(int) x+1, (int) y]) &&
                                  !availableNodes.Contains(_graph[(int) x, (int) y-1]);
        if (isBottomRightCorner)
        {
            return _graph[(int) x-1, (int) y+1]; 
        }
        var isTopBorder = !availableNodes.Contains(_graph[(int) x, (int) y + 1]);
        if (isTopBorder)
        {
            return _graph[(int) x, (int) y-1];
        }
        var isBottomBorder = !availableNodes.Contains(_graph[(int) x, (int) y - 1]);
        if (isBottomBorder)
        {
            return _graph[(int) x, (int) y+1];
        }
        var isLeftBorder = !availableNodes.Contains(_graph[(int) x-1, (int) y]);
        if (isLeftBorder)
        {
            return _graph[(int) x+1, (int) y];
        }
        var isRightBorder = !availableNodes.Contains(_graph[(int) x+1, (int) y]);
        if (isRightBorder)
        {
            return _graph[(int) x-1, (int) y];
        }
        if (availableNodes.Contains(_graph[(int)x, (int)y]))
        {
            return _graph[(int) x, (int) y];
        }
    
        return new Node {x = -1, y = -1, neighbours = new List<Node>()};
    }

    private bool IsTileAvailableFor3x3(int tileX, int tileY, HashSet<Node> availableNodes)
    {
        int startX = 0;
        int startY = 0;
        int finishX;
        int finishY;
        int unitScale = 3;
        if (tileX != 0)
        {
            startX = tileX - 1;
            finishX = startX + unitScale;
        }
        else
        {
            finishX = startX + unitScale;
        }

        if (tileY != 0)
        {
            startY = tileY - 1;
            finishY = startY + unitScale;
        }
        else
        {
            finishY = startY + unitScale;
        }

        if (tileY == _map.mapSizeY - 1)
        {
            finishY = _map.mapSizeY;
        }

        if (tileX == _map.mapSizeX - 1)
        {
            finishX = _map.mapSizeX;
        }

        for (int i = startY; i < finishY; i++)
        {
            for (int j = startX; j < finishX; j++)
            {
                if (!availableNodes.Contains(_graph[tileX, tileY]))
                {
                    return false;
                }
            }
        }
        return true;
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
                SetUnitObstacle1X1((int) unit.tileX, (int) unit.tileY, unitScale);
            }

            if (unitScale == 2)
            {
                SetUnitObstacle2X2(unit.tileX, unit.tileY);
            }

            if (unitScale == 3)
            {
                SetUnitObstacle3X3((int) unit.tileX, (int) unit.tileY, unitScale);
            }
        }
    }

    private void SetUnitObstacle1X1(int tileX, int tileY, float unitScale)
    {
        int startX = 0;
        int startY = 0;
        int finishX;
        int finishY;
        if (tileX != 0)
        {
            startX = tileX - 1;
            finishX = startX + (int) unitScale + 2;
        }
        else
        {
            finishX = startX + (int) unitScale + 1;
        }

        if (tileY != 0)
        {
            startY = tileY - 1;
            finishY = startY + (int) unitScale + 2;
        }
        else
        {
            finishY = startY + (int) unitScale + 1;
        }

        if (tileY == _map.mapSizeY - 1)
        {
            finishY = _map.mapSizeY;
        }

        if (tileX == _map.mapSizeX - 1)
        {
            finishX = _map.mapSizeX;
        }

        for (int i = startY; i < finishY; i++)
        {
            for (int j = startX; j < finishX; j++)
            {
                _dynamicObstacleNodes.Add(_graph[j, i]);
            }
        }
    }

    private void SetUnitObstacle2X2(float tileX, float tileY)
    {
        PathFindingUtils.AddObstacleNode(tileX, tileY, _dynamicObstacleNodes, _graph);
        if (tileY > 1)
        {
            _dynamicObstacleNodes.Add(_graph[(int) (tileX - 0.5f), (int) (tileY - 1.5f)]);
            _dynamicObstacleNodes.Add(_graph[(int) (tileX + 0.5f), (int) (tileY - 1.5f)]);
        }

        if (_map.mapSizeY - 1.5f > tileY)
        {
            _dynamicObstacleNodes.Add(_graph[(int) (tileX - 0.5f), (int) (tileY + 1.5f)]);
            _dynamicObstacleNodes.Add(_graph[(int) (tileX + 0.5f), (int) (tileY + 1.5f)]);
        }

        if (tileX > 1)
        {
            _dynamicObstacleNodes.Add(_graph[(int) (tileX - 1.5f), (int) (tileY + 0.5f)]);
            _dynamicObstacleNodes.Add(_graph[(int) (tileX - 1.5f), (int) (tileY - 0.5f)]);
            if (tileY > 1)
            {
                _dynamicObstacleNodes.Add(_graph[(int) (tileX - 1.5f), (int) (tileY - 1.5f)]);
            }

            if (_map.mapSizeY - 1.5f > tileY)
            {
                _dynamicObstacleNodes.Add(_graph[(int) (tileX - 1.5f), (int) (tileY + 1.5f)]);
            }
        }

        if (_map.mapSizeX - 1.5f > tileX)
        {
            _dynamicObstacleNodes.Add(_graph[(int) (tileX + 1.5f), (int) (tileY + 0.5f)]);
            _dynamicObstacleNodes.Add(_graph[(int) (tileX + 1.5f), (int) (tileY - 0.5f)]);
            if (tileY > 1)
            {
                _dynamicObstacleNodes.Add(_graph[(int) (tileX + 1.5f), (int) (tileY - 1.5f)]);
            }

            if (_map.mapSizeY - 1.5f > tileY)
            {
                _dynamicObstacleNodes.Add(_graph[(int) (tileX + 1.5f), (int) (tileY + 1.5f)]);
            }
        }
    }

    private void SetUnitObstacle3X3(int tileX, int tileY, float unitScale)
    {
        int startY = 0;
        int startX = 0;
        int finishY;
        int finishX;
        if (tileY != 1)
        {
            startY = tileY - 2;
            finishY = startY + (int) unitScale + 2;
        }
        else
        {
            finishY = startY + (int) unitScale + 1;
        }

        if (tileX != 1)
        {
            startX = tileX - 2;
            finishX = startX + (int) unitScale + 2;
        }
        else
        {
            finishX = startX + (int) unitScale + 1;
        }

        if (tileY == _map.mapSizeY - 2)
        {
            finishY = _map.mapSizeY;
        }

        if (tileX == _map.mapSizeX - 2)
        {
            finishX = _map.mapSizeX;
        }

        for (int i = startY; i < finishY; i++)
        {
            for (int j = startX; j < finishX; j++)
            {
                _dynamicObstacleNodes.Add(_graph[j, i]);
            }
        }
    }

    public List<Node> GetDynamicObstacleNodes()
    {
        return _dynamicObstacleNodes;
    }

    public void GeneratePathFindingGraph()
    {
        //Initialize the array of Nodes
        _graph = new Node[_map.mapSizeX, _map.mapSizeY];

        //Initialize a Node for each spot in the array
        for (int x = 0; x < _map.mapSizeX; x++)
        {
            for (int y = 0; y < _map.mapSizeY; y++)
            {
                _graph[x, y] = new Node {x = x, y = y};
            }
        }

        //calculate neighbours for all nodes that exist
        for (int x = 0; x < _map.mapSizeX; x++)
        {
            for (int y = 0; y < _map.mapSizeY; y++)
            {
                //Left
                if (x > 1 && _map.UnitCanEnterTile(x - 1, y, UnitSize))
                {
                    _graph[x, y].neighbours.Add(_graph[x - 1, y]);
                    if (y > 1 &&
                        _map.UnitCanEnterTile(x, y - 1, UnitSize) &&
                        _map.UnitCanEnterTile(x - 1, y, UnitSize) &&
                        _map.UnitCanEnterTile(x - 1, y - 1, UnitSize))
                        _graph[x, y].neighbours.Add(_graph[x - 1, y - 1]);
                    if (y < _map.mapSizeY - 2 &&
                        _map.UnitCanEnterTile(x, y + 1, UnitSize) &&
                        _map.UnitCanEnterTile(x - 1, y, UnitSize) &&
                        _map.UnitCanEnterTile(x - 1, y + 1, UnitSize))
                        _graph[x, y].neighbours.Add(_graph[x - 1, y + 1]);
                }

                //Right
                if (x < _map.mapSizeX - 2 && _map.UnitCanEnterTile(x + 1, y, UnitSize))
                {
                    _graph[x, y].neighbours.Add(_graph[x + 1, y]);
                    if (y > 1 &&
                        _map.UnitCanEnterTile(x, y - 1, UnitSize) &&
                        _map.UnitCanEnterTile(x + 1, y, UnitSize) &&
                        _map.UnitCanEnterTile(x + 1, y - 1, UnitSize))
                        _graph[x, y].neighbours.Add(_graph[x + 1, y - 1]);
                    if (y < _map.mapSizeX - 2 &&
                        _map.UnitCanEnterTile(x, y + 1, UnitSize) &&
                        _map.UnitCanEnterTile(x + 1, y, UnitSize) &&
                        _map.UnitCanEnterTile(x + 1, y + 1, UnitSize))
                        _graph[x, y].neighbours.Add(_graph[x + 1, y + 1]);
                }

                //Up and down
                if (y > 1 && _map.UnitCanEnterTile(x, y - 1, UnitSize))
                {
                    _graph[x, y].neighbours.Add(_graph[x, y - 1]);
                }

                if (y < _map.mapSizeX - 2 && _map.UnitCanEnterTile(x, y + 1, UnitSize))
                {
                    _graph[x, y].neighbours.Add(_graph[x, y + 1]);
                }
            }
        }
    }

    public HashSet<Node> GetAvailableNodes()
    {
        var availableNodes =  PathFindingUtils.GetAvailableNodes(_dynamicObstacleNodes, _graph, _unitManager, _map);
        return AddNodesForUnit3x3(availableNodes);
        // return  PathFindingUtils.GetAvailableNodes(_dynamicObstacleNodes, _graph, _unitManager, _map);
    }
    
    private HashSet<Node> AddNodesForUnit3x3(HashSet<Node> nodes)
    {
        var newAvailableNodes = new HashSet<Node>();
        foreach (var node in nodes)
        {
            AddNodesForUnit3x3InCollection((int) node.x, (int) node.y, newAvailableNodes);
        }

        return newAvailableNodes;
    }
    
    private void AddNodesForUnit3x3InCollection(int tileX, int tileY, HashSet<Node> nodes)
    {
        if (!_map.UnitCanEnterTile(tileX, tileY, 3))
        {
            return;
        }
        int startX = 0;
        int startY = 0;
        int finishX;
        int finishY;
        int unitScale = 3;
        if (tileX != 0)
        {
            startX = tileX - 1;
            finishX = startX + unitScale;
        }
        else
        {
            finishX = startX + unitScale;
        }

        if (tileY != 0)
        {
            startY = tileY - 1;
            finishY = startY + unitScale;
        }
        else
        {
            finishY = startY + unitScale;
        }

        if (tileY == _map.mapSizeY - 1)
        {
            finishY = _map.mapSizeY;
        }

        if (tileX == _map.mapSizeX - 1)
        {
            finishX = _map.mapSizeX;
        }

        for (int i = startY; i < finishY; i++)
        {
            for (int j = startX; j < finishX; j++)
            {
                if (_map.UnitCanEnterTile(j, i))
                {
                    nodes.Add(_graph[j, i]);
                }
            }
        }
    }
}