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
    
    public void GeneratePathTo(int x, int y)
    {
        PathFindingUtils.GeneratePathTo(x,y,_dynamicObstacleNodes,_graph,_map, _unitManager);
    }

    public void SetDynamicObstacleNodes()
    {
        _dynamicObstacleNodes = new List<Node>();
//        foreach (var uObject in _unitManager.GetUnitObjects())
//        {
//            var unit = uObject.GetComponent<Unit>();
//            var scale = unit.transform.localScale;
//            if(scale.x.Equals(1.0f) && scale.y.Equals(1.0f))
//            {
//                _dynamicObstacleNodes.Add(_graph[unit.tileX, unit.tileY]);  
//            }
//            if (scale.x > 1.0f || scale.y > 1.0f)
//            {
//                var unitScale = UIUtils.GetBiggerScale(scale.x, scale.y);
//                int startY = unit.tileY != 0 ? unit.tileY - 1 : 0;
//                int startX = unit.tileX != 0 ? unit.tileX - 1 : 0;
//                for (int i = startY; i < startY+unitScale; i++)
//                {
//                    for (int j = startX; j < startX+unitScale; j++)
//                    {
//                        _dynamicObstacleNodes.Add(_graph[j, i]); 
//                    }
//                }         
//            }
//        }
    }

    public List<Node> GetDynamicObstacleNodes()
    {
        return _dynamicObstacleNodes;
    }

    public void GeneratePathfindingGrapgh()
    {
         //Initialize the array of Nodes
        _graph = new Node[_map.mapSizeX, _map.mapSizeY];

        //Initialize a Node for each spot in the array
        for (int x = 0; x < _map.mapSizeX; x++)
        {
            for (int y = 0; y < _map.mapSizeY; y++)
            {
                _graph[x, y] = new Node();
                _graph[x, y].x = x;
                _graph[x, y].y = y;
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