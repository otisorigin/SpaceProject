using UnityEngine;

public interface ITileMap
{
    void GeneratePathTo(int x, int y);
    Vector3 TileCoordToWorldCoord(int x, int y);
    float CostToEnterTile(Node source, Node target);
}