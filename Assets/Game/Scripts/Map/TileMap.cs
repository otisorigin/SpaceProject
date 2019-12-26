using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

public class TileMap : MonoBehaviour
{
    [Inject] [NonSerialized] public GameManager manager;
    [Inject] [NonSerialized] public UnitManager _unitManager;
    [Inject] private PathFindingGraph1x1 _graph1x1;
    [Inject] private PathFindingGraph2x2 _graph2x2;
    [Inject] private PathFindingGraph3x3 _graph3x3;

    public IPathFindingGraph CurrentGraph { get; set; }

    public TileType[] tileArray;

    private int[,] _tiles;

    [NonSerialized] public int mapSizeX = 35;
    [NonSerialized] public int mapSizeY = 35;

    private Unit previousSelectedUnit;

    Node[,] _graph;

    //[Tooltip("Gameobjects not space")] private GameObject[] _gameObjects;
    //private List<Node> _dynamicObstacleNodes;
    //private List<Node> _diagonalNeighbourObstacleNodes;

    // Start is called before the first frame update
    void Start()
    {
        //SetUnitPosition();
        // SetGameObjects();
    }
    
    private void Awake()
    {
        _unitManager.OnUnitSelect += HandleUnitSelect;
    }

    public void InitTileMap()
    {
        GenerateMapData();
        GenerateMapVisual();
        _graph1x1.GeneratePathFindingGraph();
        _graph2x2.GeneratePathFindingGraph();
        _graph3x3.GeneratePathFindingGraph();
    }

    private void Update()
    {
//        if (manager.CurrentState == GameManager.GameState.UnitMovement)
//        {
//            if (previousSelectedUnit == null && manager.SelectedUnit != null)
//            {
//                previousSelectedUnit = manager.SelectedUnit;
//                SetDynamicObstacleNodes();
//            }
//            if (previousSelectedUnit != null && !previousSelectedUnit.Equals(manager.SelectedUnit))
//            {
//                SetDynamicObstacleNodes();
//                previousSelectedUnit = manager.SelectedUnit;
//            } 
//        }
    }

    private void HandleUnitSelect(Unit selectedUnit)
    {
        switch (selectedUnit.GetScale())
        {
            case 1:
                CurrentGraph = _graph1x1;
                break;
            case 2:
                CurrentGraph = _graph2x2;
                break;
            case 3:
                CurrentGraph = _graph3x3;
                break;
            default:
                CurrentGraph = _graph1x1;
                break;
        }
    }

    public float CostToEnterTile(Node source, Node target)
    {
        var tileType = tileArray[_tiles[(int)target.x, (int)target.y]];

        if (UnitCanEnterTile((int)target.x, (int)target.y) == false)
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

    public bool UnitCanEnterTile(int x, int y)
    {
        return tileArray[_tiles[x, y]].isWalkable;
    }

    public bool UnitCanEnterNode(float x, float y)
    {
        return tileArray[_tiles[(int) (x - 0.5f), (int) (y - 0.5f)]].isWalkable &&
               tileArray[_tiles[(int) (x + 0.5f), (int) (y + 0.5f)]].isWalkable &&
               tileArray[_tiles[(int) (x + 0.5f), (int) (y - 0.5f)]].isWalkable &&
               tileArray[_tiles[(int) (x - 0.5f), (int) (y + 0.5f)]].isWalkable;
    }

    public bool UnitCanEnterTile(int x, int y, int size)
    {
        int startY = 0;
        int finishY;
        int startX = 0;
        int finishX ;
        if (y != 0)
        {
            startY = y - 1;
            finishY = startY + size;
        }
        else
        {
            finishY = startY + size - 1;
        }
        if (x != 0)
        {
            startX = x - 1;
            finishX = startX + size;
        }
        else
        {
            finishX = startX + size - 1;
        }
        for (int i = startY; i < finishY; i++)
        {
            for (int j = startX; j < finishX; j++)
            {
                //Debug.Log("x = " + j + " y = " + i);
                if (j == mapSizeX || i == mapSizeY || !tileArray[_tiles[j, i]].isWalkable)
                {
                    return false;
                }
            }
        }
        return true;
    }

//    public bool UnitCanEnterTile(int x, int y)
//    {
//        return tileArray[_tiles[x, y]].isWalkable;
//    }

    public Vector3 TileCoordToWorldCoord(float x, float y)
    {
        return new Vector3(x, y, -1);
    }

    public bool IsObstaclePresentOnTile(Vector3 vector3)
    {
        return CurrentGraph.GetDynamicObstacleNodes()
            .Any(node => node.x == (int) vector3.x && node.y == (int) vector3.y);
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

        GenerateAsteroids();
        //setting test u-shape meteor barricade
//        _tiles[4, 4] = 1;
//        _tiles[5, 4] = 1;
//        _tiles[6, 4] = 1;
//        _tiles[7, 4] = 1;
//        _tiles[5, 4] = 1;
//
//        _tiles[4, 5] = 1;
//        _tiles[4, 6] = 1;
//        _tiles[8, 5] = 1;
//        _tiles[8, 6] = 1;
        /////////////////////////////////////
    }

    private void GenerateAsteroids()
    {
        var numberAsteroids = Random.Range(8, mapSizeX);
        var maxCoordAxisY = mapSizeY - _unitManager.UnitSpawnZoneLength;
        var minCoordAxisY = 0 + _unitManager.UnitSpawnZoneLength;
        for (int i = 0; i < numberAsteroids; i++)
        {
            var asteroidType = Random.Range(1, 7);
            _tiles[Random.Range(0, mapSizeX), Random.Range(minCoordAxisY, maxCoordAxisY)] = asteroidType;
        }
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
                if (_tiles[x, y] != 0)
                {
                    //clickableTile.transform.Rotate(0, 0, Random.Range(0, 360));
                }
            }
        }
    }

//    private void SetGameObjects()
//    {
//        var objects = FindObjectsOfType<GameObject>();
//        _gameObjects = objects.ToList().Where(obj => !obj.tag.Equals("Unit")).ToArray();
//    }
}