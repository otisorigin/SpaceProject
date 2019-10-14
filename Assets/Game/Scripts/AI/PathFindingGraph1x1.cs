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
        if (!_map.UnitCanEnterTile(x, y))
        {
            return;
        }
        
        var unit = _unitManager.SelectedUnit.GetComponent<Unit>();
        unit.CurrentPath = null;

        var unvisited = new List<Node>();

        var dist = new Dictionary<Node, float>();
        var prev = new Dictionary<Node, Node>();

        Node source = _graph[unit.tileX, unit.tileY];
        Node target = _graph[x, y];

        dist[source] = 0;
        prev[source] = null;

        foreach (var node in _graph)
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
             var unit = uObject.GetComponent<Unit>();
             var scale = unit.transform.localScale;
             if(scale.x.Equals(1.0f) && scale.y.Equals(1.0f))
             {
                 _dynamicObstacleNodes.Add(_graph[unit.tileX, unit.tileY]);  
             }
             if (scale.x > 1.0f || scale.y > 1.0f)
             {
                 var unitScale = UIUtils.GetBiggerScale(scale.x, scale.y);
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