using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class TileMap : MonoBehaviour
{
    [Inject] [NonSerialized] private GameManager manager;
    [Inject] [NonSerialized] private UnitManager _unitManager;
    [Inject] private PathFindingGraph1x1 _graph1x1;
    [Inject] private PathFindingGraph2x2 _graph2x2;
    [Inject] private PathFindingGraph3x3 _graph3x3;
    public IPathFindingGraph CurrentGraph { get; set; }

    public TileType[] tileArray;
    public SpriteRenderer[] nebulaArray;
    public SpriteRenderer[] planetArray;
    private int[,] _tiles;
    private List<ClickableTile> _availablePathTiles;
    public int minAsteroidsNumber = 13;

    [NonSerialized] public int mapSizeX = 35;
    [NonSerialized] public int mapSizeY = 35;

    Node[,] _graph;
    private ClickableTile[,] _pathTilePool;

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

        if (!UnitCanEnterTile((int)target.x, (int)target.y))
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

    public void ShowAvailablePathTiles(HashSet<Node> nodes)
    {    
        ClearAvailablePathTiles();
        foreach (var node in nodes)
        {
            var pathTile = _pathTilePool[(int)node.x, (int)node.y];
            pathTile.gameObject.SetActive(true);
            _availablePathTiles.Add(pathTile);
        }
    }

    private void ClearAvailablePathTiles()
    {
        foreach (var clickableTile in _pathTilePool)
        {
            clickableTile.gameObject.SetActive(false);
        }
        _availablePathTiles.Clear();
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
        _availablePathTiles = new List<ClickableTile>();
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                _tiles[x, y] = 0;
            }
        }
        CreatePathTilePool();
        GenerateAsteroids();
    }

    private void CreatePathTilePool()
    {
        _pathTilePool = new ClickableTile[mapSizeX,mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                _pathTilePool[x, y] = Instantiate(tileArray[8].tileVisualPrefab, new Vector3(x, y, 1), Quaternion.identity);
                _pathTilePool[x, y].gameObject.SetActive(false);
            }
        }
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
                
                ClickableTile clickableTile = square;
                clickableTile.tileX = x;
                clickableTile.tileY = y;
                clickableTile.Map = this;
                clickableTile.GameManager = manager;
                clickableTile.UnitManager = _unitManager;
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
}