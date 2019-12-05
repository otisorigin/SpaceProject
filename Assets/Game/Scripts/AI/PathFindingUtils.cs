using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingUtils
{
    public static void GeneratePathTo(int x, int y, List<Node> dynamicObstacleNodes, Node[,] graph, TileMap map, UnitManager unitManager)
    {
         if (!map.UnitCanEnterTile(x, y))
        {
            return;
        }
        
        var unit = unitManager.SelectedUnit.GetComponent<Unit>();
        unit.CurrentPath = null;

        var unvisited = new List<Node>();

        var dist = new Dictionary<Node, float>();
        var prev = new Dictionary<Node, Node>();

        Node source = graph[(int)unit.tileX, (int)unit.tileY];
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

            if (!dynamicObstacleNodes.Contains(node))
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
                if (!dynamicObstacleNodes.Contains(v) /*&& !_diagonalNeighbourObstacleNodes.Contains(v)*/)
                {
                    float alt = dist[u] + map.CostToEnterTile(u, v);
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

    public static void AddObstacleNode(float x, float y, List<Node> obstacles, Node[,] graph)
    {
        obstacles.Add(graph[(int)(x-0.5f), (int)(y-0.5f)]);  
        obstacles.Add(graph[(int)(x+0.5f), (int)(y+0.5f)]);  
        obstacles.Add(graph[(int)(x-0.5f), (int)(y+0.5f)]);
        obstacles.Add(graph[(int)(x+0.5f), (int)(y-0.5f)]);
    }
}