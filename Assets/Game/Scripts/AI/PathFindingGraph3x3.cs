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
        PathFindingUtils.GeneratePathTo((int)x,(int)y,_dynamicObstacleNodes,_graph,_map, _unitManager);
    }

    public void SetDynamicObstacleNodes()
    {
        _dynamicObstacleNodes = new List<Node>();
        foreach (var uObject in _unitManager.GetUnitObjects())
        {
            var unit = uObject.GetComponent<Unit>();
            var scale = uObject.transform.GetChild(0).transform.localScale;
            var unitScale = UIUtils.GetBiggerScale(scale.x, scale.y);
            if(unitScale.Equals(1.0f))
            {
                SetUnitObstacle1X1((int)unit.tileX, (int)unit.tileY, unitScale);
            }
           
            //TODO заглушка пока не реализуются нормально юниты 2х2
            ////////////////////////////////////////////////////
            if (unitScale.Equals(2.0f))
            {
                continue;
            }
            //////////////////////////////////////////////////////
            if (unitScale.Equals(3.0f))
            {
                SetUnitObstacle3X3((int)unit.tileX, (int)unit.tileY, unitScale);
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
            finishX = startX + (int)unitScale + 2;
        }
        else
        {
            finishX = startX + (int) unitScale + 1;
        }
        if (tileY != 0)
        {
            startY = tileY - 1;
            finishY = startY + (int)unitScale + 2;
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

    private void SetUnitObstacle3X3(int tileX, int tileY, float unitScale)
    {
        int startY = 0;
        int startX = 0;
        int finishY;
        int finishX;
        if (tileY != 1)
        {
            startY = tileY - 2;
            finishY = startY + (int)unitScale + 2;
        }
        else
        {
            finishY = startY + (int) unitScale + 1;
        }
        if (tileX != 1)
        {
            startX = tileX - 2;
            finishX = startX + (int)unitScale + 2;
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
                if (x > 1 && _map.UnitCanEnterTile(x-1, y,UnitSize))
                {
                    _graph[x, y].neighbours.Add(_graph[x - 1, y]);
                        if (y > 1 && 
                            _map.UnitCanEnterTile(x, y - 1,UnitSize) &&
                            _map.UnitCanEnterTile(x - 1, y,UnitSize) &&
                            _map.UnitCanEnterTile(x - 1, y - 1,UnitSize))
                                _graph[x, y].neighbours.Add(_graph[x - 1, y - 1]);
                        if (y < _map.mapSizeY - 2 &&
                            _map.UnitCanEnterTile(x, y + 1,UnitSize) &&
                            _map.UnitCanEnterTile(x - 1, y,UnitSize) &&
                            _map.UnitCanEnterTile(x - 1, y + 1,UnitSize))
                                _graph[x, y].neighbours.Add(_graph[x - 1, y + 1]);
                }

                //Right
                if (x < _map.mapSizeX - 2 && _map.UnitCanEnterTile(x + 1, y,UnitSize))
                {
                    _graph[x, y].neighbours.Add(_graph[x + 1, y]);
                        if (y > 1 && 
                            _map.UnitCanEnterTile(x, y - 1,UnitSize) &&
                            _map.UnitCanEnterTile(x + 1, y,UnitSize) &&
                            _map.UnitCanEnterTile(x + 1, y - 1,UnitSize))
                                _graph[x, y].neighbours.Add(_graph[x + 1, y - 1]);
                        if (y < _map.mapSizeX - 2 && 
                            _map.UnitCanEnterTile(x, y + 1,UnitSize) &&
                            _map.UnitCanEnterTile(x + 1, y,UnitSize) &&
                            _map.UnitCanEnterTile(x + 1, y + 1,UnitSize))
                                _graph[x, y].neighbours.Add(_graph[x + 1, y + 1]);
                }

                //Up and down
                if (y > 1 && _map.UnitCanEnterTile(x, y-1,UnitSize))
                {
                    _graph[x, y].neighbours.Add(_graph[x, y - 1]);
                }

                if (y < _map.mapSizeX - 2 && _map.UnitCanEnterTile(x, y+1,UnitSize))
                {
                    _graph[x, y].neighbours.Add(_graph[x, y + 1]);
                }
            }
        }
    }
}