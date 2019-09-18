﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class TileMap : MonoBehaviour
{
    [FormerlySerializedAs("_controller")] [Inject]
    public GameManager manager;

    public TileType[] tileArray;

    private int[,] _tiles;

    public int mapSizeX = 25;
    public int mapSizeY = 25;

    Node[,] _graph;

    //[Tooltip("Gameobjects not space")] private GameObject[] _gameObjects;
    private List<Node> _dynamicObstacleNodes;

    // Start is called before the first frame update
    void Start()
    {
        SetUnitPosition();
       // SetGameObjects();
        GenerateMapData();
        GeneratePathfindingGrapgh();
        GenerateMapVisual();
        _dynamicObstacleNodes = new List<Node>();
        _dynamicObstacleNodes.Add(_graph[2,2]);
        _dynamicObstacleNodes.Add(_graph[1,1]);
        _dynamicObstacleNodes.Add(_graph[1,2]);
        _dynamicObstacleNodes.Add(_graph[1,3]);
        _dynamicObstacleNodes.Add(_graph[2,1]);
        _dynamicObstacleNodes.Add(_graph[2,3]);
        _dynamicObstacleNodes.Add(_graph[3,1]);
        _dynamicObstacleNodes.Add(_graph[3,2]);
        _dynamicObstacleNodes.Add(_graph[3,3]);
    }

//    public void UnitSelect(Unit unit)
//    {
////        if (GetSelectedUnit() != null)
////        {
////            GetSelectedUnit().isSelected = false;
////        }
////        _unitGroup.SelectedUnit  = unit;
////        GetSelectedUnit().isSelected = true;
//    }    

//    public GameObject GetObjectFromCoord(int x, int y)
//    {
//        foreach (GameObject go in _gameObjects)
//        {
//            if (go.transform.position.x.Equals(x) && go.transform.position.y.Equals(y))
//            {
//                return go;
//            }
//        }
//
//        Debug.Log("return null");
//        return null;
//    }

    private void SetDynamicObstacleNodes()
    {
        _dynamicObstacleNodes = new List<Node>();
        var unitObjects = FindObjectsOfType<GameObject>().ToList().Where(obj => obj.tag.Equals("Unit")).ToArray();
        foreach (var uObject in unitObjects)
        {
            var unit = uObject.GetComponent<Unit>();
            var scale = unit.transform.localScale;
            if(scale.x == 1.0f && scale.y == 1.0f)
            {
                _dynamicObstacleNodes.Add(_graph[unit.tileX, unit.tileY]);  
            }
            if (scale.x > 1.0f || scale.y > 1.0f)
            {
                var unitScale = UIUtils.GetBiggerScale(scale.x, scale.y);
                for(int i = unit.tileY != 0 ? unit.tileY-1 : 0; i < unitScale; i++)
                    for (int j = unit.tileX != 0 ? unit.tileX-1 : 0; j < unitScale; j++)
                        _dynamicObstacleNodes.Add(_graph[j, i]);
            }
        }
    }

    public float CostToEnterTile(Node source, Node target)
    {
        var tileType = tileArray[_tiles[target.x, target.y]];

        if (UnitCanEnterTile(target.x, target.y) == false)
        {
            return Mathf.Infinity;
        }

        float cost = tileType.movementCost;

        if (source.x != target.x && source.y != target.y)
        {
            //We are moving diagonally! Fudge the cost for tile-braking
            cost += 1.0f;
        }

        return cost;
    }

    public void GeneratePathTo(int x, int y)
    {
        if (!UnitCanEnterTile(x, y))
        {
            return;
        }
        
        //SetDynamicObstacleNodes();
        var unit = manager.SelectedUnit.GetComponent<Unit>();
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

            //unvisited.Add(node);
            if (/*node != source && */!_dynamicObstacleNodes.Contains(node))
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
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + CostToEnterTile(u, v);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
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

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, -1);
    }

    private void SetUnitPosition()
    {
        if (manager.SelectedUnit != null)
        {
            var unit = manager.SelectedUnit.GetComponent<Unit>();
            unit.tileX = (int) unit.transform.position.x;
            unit.tileY = (int) unit.transform.position.y;
            // unit.map = this;
        }
    }

    private void GeneratePathfindingGrapgh()
    {
        //Initialize the array of Nodes
        _graph = new Node[mapSizeX, mapSizeY];

        //Initialize a Node for each spot in the array
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                _graph[x, y] = new Node();
                _graph[x, y].x = x;
                _graph[x, y].y = y;
            }
        }

        //calculate neighbours for all nodes that exist
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                //Left
                if (x > 0)
                {
                    _graph[x, y].neighbours.Add(_graph[x - 1, y]);
                    if (y > 0)
                        if (UnitCanEnterTile(x, y - 1))
                            if (UnitCanEnterTile(x - 1, y))
                                _graph[x, y].neighbours.Add(_graph[x - 1, y - 1]);
                    if (y < mapSizeY - 1)
                        if (UnitCanEnterTile(x, y + 1))
                            if (UnitCanEnterTile(x - 1, y))
                                _graph[x, y].neighbours.Add(_graph[x - 1, y + 1]);
                }

                //Right
                if (x < mapSizeX - 1)
                {
                    _graph[x, y].neighbours.Add(_graph[x + 1, y]);
                    if (y > 0)
                        if (UnitCanEnterTile(x, y - 1))
                            if (UnitCanEnterTile(x + 1, y))
                                _graph[x, y].neighbours.Add(_graph[x + 1, y - 1]);
                    if (y < mapSizeX - 1)
                        if (UnitCanEnterTile(x, y + 1))
                            if (UnitCanEnterTile(x + 1, y))
                                _graph[x, y].neighbours.Add(_graph[x + 1, y + 1]);
                }

                //Up and down
                if (y > 0)
                {
                    _graph[x, y].neighbours.Add(_graph[x, y - 1]);
                }

                if (y < mapSizeX - 1)
                {
                    _graph[x, y].neighbours.Add(_graph[x, y + 1]);
                }
            }
        }
    }

    private void GenerateMapData()
    {
        _tiles = new int[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                _tiles[x, y] = 0;
            }
        }

        //setting test u-shape meteor barricade
        _tiles[4, 4] = 1;
        _tiles[5, 4] = 1;
        _tiles[6, 4] = 1;
        _tiles[7, 4] = 1;
        _tiles[5, 4] = 1;

        _tiles[4, 5] = 1;
        _tiles[4, 6] = 1;
        _tiles[8, 5] = 1;
        _tiles[8, 6] = 1;
        /////////////////////////////////////
    }

    private void GenerateMapVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                var tileType = tileArray[_tiles[x, y]];

                var square = Instantiate(tileType.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity);

                ClickableTile clickableTile = square.GetComponent<ClickableTile>();
                clickableTile.tileX = x;
                clickableTile.tileY = y;
                clickableTile.Map = this;
            }
        }
    }

//    private void SetGameObjects()
//    {
//        var objects = FindObjectsOfType<GameObject>();
//        _gameObjects = objects.ToList().Where(obj => !obj.tag.Equals("Unit")).ToArray();
//    }

    private bool UnitCanEnterTile(int x, int y)
    {
        return tileArray[_tiles[x, y]].isWalkable;
    }
}