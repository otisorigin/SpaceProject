using System;
using System.Collections;
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
    public SpriteRenderer[] nebulaArray;
    public SpriteRenderer[] planetArray;

    private int[,] _tiles;
    private int minAsteroidsNumber = 13;

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
    }

    private void GenerateAsteroids()
    {
        var numberAsteroids = Random.Range(minAsteroidsNumber, mapSizeX);
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
        GenerateTileVisual();
        GenerateNebulaObjects();
        GeneratePlanetsObjects();
    }

    private void GenerateTileVisual()
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

    private void GenerateNebulaObjects()
    {
        var objectsNumber = Random.Range(0,6);
        for (int i = 0; i < objectsNumber; i++)
        {
            var objectType = nebulaArray[Random.Range(0, nebulaArray.Length-1)];
            var xCoord = Random.Range(-3.0f, mapSizeX+3);
            var yCoord = Random.Range(-3.0f, mapSizeY+3);
            Instantiate(objectType, new Vector3(xCoord, yCoord, 2), Quaternion.identity);
        }
    }

    private void GeneratePlanetsObjects()
    {
        var planetsNumber = Random.Range(0,5);
        var planets = planetArray.ToList();
        for (int i = 1; i <= planetsNumber; i++)
        {
            var index = Random.Range(0,planets.Count - 1);
            var planetType = planets[index];
            planets.RemoveAt(index);
            var xCoord = Random.Range(GetMinPlanetCoord(i,planetsNumber) , GetMaxPlanetCoord(i, planetsNumber));
            var yCoord = Random.Range(GetMinPlanetCoord(i,planetsNumber) , GetMaxPlanetCoord(i, planetsNumber));
            var scale = Random.Range(0.3f, 1.2f);
            planetType.transform.localScale = new Vector3(scale,scale,planetType.transform.localScale.z);
            Instantiate(planetType, new Vector3(xCoord, yCoord, 2), Quaternion.identity);
        }
    }

    private float GetMinPlanetCoord(int iterator, int planetsNumber)
    {
        return -3.0f/planetsNumber*Random.Range((float)iterator,planetsNumber);
    }
    
    private float GetMaxPlanetCoord(int iterator, int planetsNumber)
    {
        return (mapSizeX+3.0f)/planetsNumber*Random.Range((float)iterator,planetsNumber);
    }

//    private void SetGameObjects()
//    {
//        var objects = FindObjectsOfType<GameObject>();
//        _gameObjects = objects.ToList().Where(obj => !obj.tag.Equals("Unit")).ToArray();
//    }
}