using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public List<Node> neighbours;
    public float x;
    public float y;

    public Node()
    {
        neighbours = new List<Node>();
    }

    public float DistanceTo(Node node)
    {
        return Vector2.Distance(
            new Vector2(x, y),
            new Vector2(node.x, node.y)
        );
    }
}
