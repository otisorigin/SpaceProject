using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingUtils
{
    public static void GeneratePathTo(Node source, Node target, List<Node> dynamicObstacleNodes, /*Node[,] graph,*/ TileMap map,
        UnitManager unitManager)
    {
        Debug.Log("Target X: " + target.x + " Y: " + target.y);
        var unit = unitManager.GetSelectedUnitMovementSystem();
        unit.CurrentPath = null;
        var path = AStarSearch(source, target, unit.RemainingMovement, dynamicObstacleNodes, map);

        //we found shortest way to our target or there is no way to our target
        if (path[target] == null)
        {
            //no way between our target and the source
            return;
        }

        var currentPath = new List<Node>();

        Node curr = target;

        while (curr != null)
        {
            currentPath.Add(curr);
            curr = path[curr];
        }

        currentPath.Reverse();
        unit.CurrentPath = currentPath;
    }

    public static Dictionary<Node, Node> AStarSearch(Node source, Node target, int remainingMovement, List<Node> dynamicObstacleNodes, TileMap map)
    {
        var cameFrom = new Dictionary<Node, Node>();
        var costSoFar = new Dictionary<Node, float>();
        
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(source, 0);

        cameFrom[source] = null;
        cameFrom[target] = null;
        costSoFar[source] = 0;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            
            if (current.Equals(target))
            {
                break;
            }
            
            foreach (var next in current.neighbours)
            {
                if (!dynamicObstacleNodes.Contains(next))
                {
                    float newCost = costSoFar[current] + map.CostToEnterTile(current, next);
                    if (newCost <= remainingMovement && (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]))
                    {
                        costSoFar[next] = newCost;
                        double priority = newCost + Heuristic(next, target);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    } 
                }
            }
        }
        return cameFrom;
    }

    private static double Heuristic(Node a, Node b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    public static HashSet<Node> GetAvailableNodes(List<Node> dynamicObstacleNodes, Node[,] graph, UnitManager unitManager,
        TileMap map)
    {
        var unit = unitManager.GetSelectedUnitMovementSystem();
        var unitScale = unitManager.SelectedUnit.GetScale();
        if (unit.RemainingMovement == 0)
        {
            return new HashSet<Node>();
        }

        var unvisited = new HashSet<Node>();

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

        var availableNodes = new HashSet<Node>();

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
                if (!dynamicObstacleNodes.Contains(v) && map.UnitCanEnterTile((int)u.x, (int)u.y, unitScale))
                {
                    float alt = dist[u] + map.CostToEnterTile(u, v);
                    if (unit.RemainingMovement + 2 < alt && !float.IsPositiveInfinity(map.CostToEnterTile(u, v)))
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

        return availableNodes; 
    }
    
    

    public static void AddObstacleNode(float x, float y, List<Node> obstacles, Node[,] graph)
    {
        obstacles.Add(graph[(int) (x - 0.5f), (int) (y - 0.5f)]);
        obstacles.Add(graph[(int) (x + 0.5f), (int) (y + 0.5f)]);
        obstacles.Add(graph[(int) (x - 0.5f), (int) (y + 0.5f)]);
        obstacles.Add(graph[(int) (x + 0.5f), (int) (y - 0.5f)]);
    }

    public static bool IsRemainingMovementEnoughWithCost(Node source, Node target, int remainingMovement)
    {
        var delta = Math.Abs(source.y - target.y);
        return !(target.y - source.y <= remainingMovement) ||
               !(target.x - source.x <= remainingMovement - delta) ||
               !(target.y - source.y >= -remainingMovement) ||
               !(target.x - source.x >= -remainingMovement + delta);
    }
}