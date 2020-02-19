using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PathFindingUtils
{
    public static void GeneratePathTo(int x, int y, List<Node> dynamicObstacleNodes, Node[,] graph, TileMap map,
        UnitManager unitManager)
    {
        var unit = unitManager.GetSelectedUnitMovementSystem();
        unit.CurrentPath = null;

        if (unit.RemainingMovement == 0 || !map.UnitCanEnterTile(x, y) ||
            IsRemainingMovementEnoughWithCost(graph[(int) unit.tileX, (int) unit.tileY], graph[x, y],
                unit.RemainingMovement))
        {
            return;
        }

        var unvisited = new List<Node>();

        var dist = new Dictionary<Node, float>();
        var prev = new Dictionary<Node, Node>();

        Node source = graph[(int) unit.tileX, (int) unit.tileY];
        Node target = graph[x, y];

        dist[source] = 0;
        prev[source] = null;

        foreach (var node in graph)
        {
            if (node != source)
            {
                dist[node] = Mathf.Infinity;
                prev[node] = null;
            }

            if (!dynamicObstacleNodes.Contains(node) &&
                !IsRemainingMovementEnoughWithCost(source, node, unit.RemainingMovement))
            {
                unvisited.Add(node);
            }
        }

        var availableNodes = new List<Node>();

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
                if (!dynamicObstacleNodes.Contains(v) /*&& !_diagonalNeighbourObstacleNodes.Contains(v)*/)
                {
                    float alt = dist[u] + map.CostToEnterTile(u, v);
                   
                    if (alt < dist[v] && alt <= unit.RemainingMovement)
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }

                    
                }
            }
        }

//        map.ShowAvailableTilesToMove(availableNodes);

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
        //map.ShowAvailableTilesToMove(availableNodes);
    }

    public static List<Node> GetAvailableNodes(List<Node> dynamicObstacleNodes, Node[,] graph, UnitManager unitManager,
        TileMap map)
    {
        var unit = unitManager.GetSelectedUnitMovementSystem();

        if (unit.RemainingMovement == 0)
        {
            return new List<Node>();
        }

        var unvisited = new List<Node>();

        var dist = new Dictionary<Node, float>();
        var prev = new Dictionary<Node, Node>();

        Node source = graph[(int) unit.tileX, (int) unit.tileY];

        dist[source] = 0;
        prev[source] = null;

        foreach (var node in graph)
        {
            if (node != source)
            {
                dist[node] = Mathf.Infinity;
                prev[node] = null;
            }

            if (!dynamicObstacleNodes.Contains(node) &&
                !IsRemainingMovementEnoughWithCost(source, node, unit.RemainingMovement))
            {
                unvisited.Add(node);
            }
        }

        var availableNodes = new List<Node>();

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

            unvisited.Remove(u);
            availableNodes.Add(u);

            foreach (var v in u.neighbours)
            {
                if (!dynamicObstacleNodes.Contains(v))
                {
                    float alt = dist[u] + map.CostToEnterTile(u, v);
                    if (unit.RemainingMovement+2 < alt && !float.IsPositiveInfinity(map.CostToEnterTile(u, v)))
                    {
                        availableNodes.Remove(u);
                    }
                    if (alt < dist[v] && alt <= unit.RemainingMovement)
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }
        }
       // map.ShowAvailableTilesToMove(availableNodes);
        return availableNodes;
    }

    public static void AddObstacleNode(float x, float y, List<Node> obstacles, Node[,] graph)
    {
        obstacles.Add(graph[(int) (x - 0.5f), (int) (y - 0.5f)]);
        obstacles.Add(graph[(int) (x + 0.5f), (int) (y + 0.5f)]);
        obstacles.Add(graph[(int) (x - 0.5f), (int) (y + 0.5f)]);
        obstacles.Add(graph[(int) (x + 0.5f), (int) (y - 0.5f)]);
    }

//    private static bool IsRemainingMovementEnough(Node source, Node target, int remainingMovement)
//    {
//        return !(target.y - source.y <= remainingMovement) ||
//               !(target.x - source.x <= remainingMovement) ||
//               !(target.y - source.y >= -remainingMovement) ||
//               !(target.x - source.x >= -remainingMovement);
//    }

    private static bool IsRemainingMovementEnoughWithCost(Node source, Node target, int remainingMovement)
    {
        var delta = Math.Abs(source.y - target.y);
        return !(target.y - source.y <= remainingMovement) ||
               !(target.x - source.x <= remainingMovement - delta) ||
               !(target.y - source.y >= -remainingMovement) ||
               !(target.x - source.x >= -remainingMovement + delta);
    }
}