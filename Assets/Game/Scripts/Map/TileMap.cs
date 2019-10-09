using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class TileMap : MonoBehaviour
{
    [Inject] public GameManager manager;
    [Inject] public UnitManager unitManager;
    [Inject] private PathFindingGraph1x1 _graph1x1;
    [Inject] private PathFindingGraph2x2 _graph2x2;
    [Inject] private PathFindingGraph3x3 _graph3x3;

    public IPathFindingGraph CurrentGraph { get; set; }

    public TileType[] tileArray;

    private int[,] _tiles;

    public int mapSizeX = 25;
    public int mapSizeY = 25;

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

    public void InitTileMap()
    {
        GenerateMapData();
        GenerateMapVisual(); 
        _graph1x1.GeneratePathfindingGrapgh();
        _graph2x2.GeneratePathfindingGrapgh();
        _graph3x3.GeneratePathfindingGrapgh();
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

    public void SetCurrentGraph(int size)
    {
        switch (size)
        {
            case 1: CurrentGraph = _graph1x1; break;
            case 2: CurrentGraph = _graph2x2; break;
            case 3: CurrentGraph = _graph3x3; break;
            default: CurrentGraph = _graph1x1; break;
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
    
    public bool UnitCanEnterTile(int x, int y)
    {
        return tileArray[_tiles[x, y]].isWalkable;
    }
    
    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, -1);
    }
    
    public bool IsObstaclePresentOnTile(Vector3 vector3)
    {
        return CurrentGraph.GetDynamicObstacleNodes().Any(node => node.x == (int)vector3.x && node.y == (int)vector3.y);
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

//    public void GeneratePathTo(int x, int y)
//    {
//        if (!UnitCanEnterTile(x, y))
//        {
//            return;
//        }
//        
//        var unit = manager.SelectedUnit.GetComponent<Unit>();
//        unit.CurrentPath = null;
//
//        var unvisited = new List<Node>();
//
//        var dist = new Dictionary<Node, float>();
//        var prev = new Dictionary<Node, Node>();
//
//        Node source = _graph[unit.tileX, unit.tileY];
//        Node target = _graph[x, y];
//
//        dist[source] = 0;
//        prev[source] = null;
//
//        foreach (var node in _graph)
//        {
//            if (node != source)
//            {
//                dist[node] = Mathf.Infinity;
//                prev[node] = null;
//            }
//
//            if (!_dynamicObstacleNodes.Contains(node))
//            {
//                unvisited.Add(node);
//            }
//        }
//
//        while (unvisited.Count > 0)
//        {
//            //unvisited node with smallest distance
//            Node u = null;
//
//            foreach (var possibleU in unvisited)
//            {
//                if (u == null || dist[possibleU] < dist[u])
//                {
//                    u = possibleU;
//                }
//            }
//
//            if (u == target)
//            {
//                break;
//            }
//
//            unvisited.Remove(u);
//
//            foreach (var v in u.neighbours)
//            {
//                if (!_dynamicObstacleNodes.Contains(v) /*&& !_diagonalNeighbourObstacleNodes.Contains(v)*/)
//                {
//                    float alt = dist[u] + CostToEnterTile(u, v);
//                    if (alt < dist[v])
//                    {
//                        dist[v] = alt;
//                        prev[v] = u;
//                    } 
//                }
//                
//            }
//        }
//
//        //we found shortest way to our target or there is no way to our target
//        if (prev[target] == null)
//        {
//            //no way between our target and the source
//            return;
//        }
//
//        var currentPath = new List<Node>();
//
//        Node curr = target;
//
//        while (curr != null)
//        {
//            currentPath.Add(curr);
//            curr = prev[curr];
//        }
//
//        currentPath.Reverse();
//        unit.CurrentPath = currentPath;
//    }

    
    
    private void SetDynamicObstacleNodes()
    {
//        _dynamicObstacleNodes = new List<Node>();
//       // _diagonalNeighbourObstacleNodes = new List<Node>();
//        var unitObjects = FindObjectsOfType<GameObject>()
//            .ToList()
//            .Where(obj => obj.tag.Equals("Unit"))
//            .Where(unit => !unit.GetComponent<Unit>().Equals(manager.SelectedUnit));
//        foreach (var uObject in unitObjects)
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
        //}
//        _dynamicObstacleNodes.ForEach(SetDiagonalNeighbours);
//        Debug.Log("-----------Start--------------");
//        _diagonalNeighbourObstacleNodes.ForEach(node => Debug.Log("Node x: " + node.x + " y: " + node.y + "\n"));
//        Debug.Log("-----------End-------------------");
    }

//    private void SetDiagonalNeighbours(Node node)
//    {
//        node.neighbours.ForEach(neighbour => TrySetDiagonalNeighbour(node, neighbour));
//    }
    
//    private void TrySetDiagonalNeighbour(Node sourceNode, Node neighbourNode)
//    {
//        var diagonalLeftDownCondition = sourceNode.x - 1 == neighbourNode.x && sourceNode.y - 1 == neighbourNode.y;
//        var diagonalLeftUpCondition = sourceNode.x - 1 == neighbourNode.x && sourceNode.y + 1 == neighbourNode.y;
//        var diagonalRightUpCondition = sourceNode.x + 1 == neighbourNode.x && sourceNode.y + 1 == neighbourNode.y;
//        var diagonalRightDownCondition = sourceNode.x + 1 == neighbourNode.x && sourceNode.y - 1 == neighbourNode.y;
//        if (diagonalLeftDownCondition || diagonalLeftUpCondition ||
//            diagonalRightUpCondition || diagonalRightDownCondition)
//        {
//            _diagonalNeighbourObstacleNodes.Add(neighbourNode);
//        }
//    }

//    private void SetUnitPosition()
//    {
//        if (manager.SelectedUnit != null)
//        {
//            var unit = manager.SelectedUnit.GetComponent<Unit>();
//            unit.tileX = (int) unit.transform.position.x;
//            unit.tileY = (int) unit.transform.position.y;
//            // unit.map = this;
//        }
//    }

//    private void GeneratePathfindingGrapgh()
//    {
//        //Initialize the array of Nodes
//        _graph = new Node[mapSizeX, mapSizeY];
//
//        //Initialize a Node for each spot in the array
//        for (int x = 0; x < mapSizeX; x++)
//        {
//            for (int y = 0; y < mapSizeY; y++)
//            {
//                _graph[x, y] = new Node();
//                _graph[x, y].x = x;
//                _graph[x, y].y = y;
//            }
//        }
//
//        //calculate neighbours for all nodes that exist
//        for (int x = 0; x < mapSizeX; x++)
//        {
//            for (int y = 0; y < mapSizeY; y++)
//            {
//                //Left
//                if (x > 0)
//                {
//                    _graph[x, y].neighbours.Add(_graph[x - 1, y]);
//                    if (y > 0)
//                        if (UnitCanEnterTile(x, y - 1))
//                            if (UnitCanEnterTile(x - 1, y))
//                                _graph[x, y].neighbours.Add(_graph[x - 1, y - 1]);
//                    if (y < mapSizeY - 1)
//                        if (UnitCanEnterTile(x, y + 1))
//                            if (UnitCanEnterTile(x - 1, y))
//                                _graph[x, y].neighbours.Add(_graph[x - 1, y + 1]);
//                }
//
//                //Right
//                if (x < mapSizeX - 1)
//                {
//                    _graph[x, y].neighbours.Add(_graph[x + 1, y]);
//                    if (y > 0)
//                        if (UnitCanEnterTile(x, y - 1))
//                            if (UnitCanEnterTile(x + 1, y))
//                                _graph[x, y].neighbours.Add(_graph[x + 1, y - 1]);
//                    if (y < mapSizeX - 1)
//                        if (UnitCanEnterTile(x, y + 1))
//                            if (UnitCanEnterTile(x + 1, y))
//                                _graph[x, y].neighbours.Add(_graph[x + 1, y + 1]);
//                }
//
//                //Up and down
//                if (y > 0)
//                {
//                    _graph[x, y].neighbours.Add(_graph[x, y - 1]);
//                }
//
//                if (y < mapSizeX - 1)
//                {
//                    _graph[x, y].neighbours.Add(_graph[x, y + 1]);
//                }
//            }
//        }
//    }

    

//    private void SetGameObjects()
//    {
//        var objects = FindObjectsOfType<GameObject>();
//        _gameObjects = objects.ToList().Where(obj => !obj.tag.Equals("Unit")).ToArray();
//    }

    
    
}