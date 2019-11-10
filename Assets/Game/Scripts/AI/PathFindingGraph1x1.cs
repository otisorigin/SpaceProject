using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PathFindingGraph1x1 : MonoBehaviour, IPathFindingGraph
{
    [Inject] private TileMap _map;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;

    private List<Node> _dynamicObstacleNodes;
    Node[,] _graph;
    
    public void GeneratePathTo(int x, int y)
    {
        PathFindingUtils.GeneratePathTo(x,y,_dynamicObstacleNodes,_graph,_map, _unitManager);
    }
     
     public void SetDynamicObstacleNodes()
     {
         _dynamicObstacleNodes = new List<Node>();
         foreach (var uObject in _unitManager.GetUnitObjects())
         {
             var unit = uObject.GetComponent<Unit>();
             var scale = unit.transform.localScale;
             var unitScale = UIUtils.GetBiggerScale(scale.x, scale.y);
             if(unitScale.Equals(1.0f))
             {
                 _dynamicObstacleNodes.Add(_graph[unit.tileX, unit.tileY]);  
             }
             if (unitScale > 1.0f)
             {
                 //TODO возможно тут ещё стоит проверить по верхней границе? (mapSize)
                 int startY = unit.tileY != 0 ? unit.tileY - 1 : 0;
                 int startX = unit.tileX != 0 ? unit.tileX - 1 : 0;
                 for (int i = startY; i < startY+unitScale; i++)
                 {
                     for (int j = startX; j < startX+unitScale; j++)
                     {
                         _dynamicObstacleNodes.Add(_graph[j, i]); 
                     }
                 }         
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
                if (x > 0)
                {
                    _graph[x, y].neighbours.Add(_graph[x - 1, y]);
                    if (y > 0)
                        if (_map.UnitCanEnterTile(x, y - 1))
                            if (_map.UnitCanEnterTile(x - 1, y))
                                _graph[x, y].neighbours.Add(_graph[x - 1, y - 1]);
                    if (y < _map.mapSizeY - 1)
                        if (_map.UnitCanEnterTile(x, y + 1))
                            if (_map.UnitCanEnterTile(x - 1, y))
                                _graph[x, y].neighbours.Add(_graph[x - 1, y + 1]);
                }

                //Right
                if (x < _map.mapSizeX - 1)
                {
                    _graph[x, y].neighbours.Add(_graph[x + 1, y]);
                    if (y > 0)
                        if (_map.UnitCanEnterTile(x, y - 1))
                            if (_map.UnitCanEnterTile(x + 1, y))
                                _graph[x, y].neighbours.Add(_graph[x + 1, y - 1]);
                    if (y < _map.mapSizeX - 1)
                        if (_map.UnitCanEnterTile(x, y + 1))
                            if (_map.UnitCanEnterTile(x + 1, y))
                                _graph[x, y].neighbours.Add(_graph[x + 1, y + 1]);
                }

                //Up and down
                if (y > 0)
                {
                    _graph[x, y].neighbours.Add(_graph[x, y - 1]);
                }

                if (y < _map.mapSizeX - 1)
                {
                    _graph[x, y].neighbours.Add(_graph[x, y + 1]);
                }
            }
        }
    }
}