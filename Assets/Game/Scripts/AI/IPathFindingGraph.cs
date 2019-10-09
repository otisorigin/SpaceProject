

using System.Collections.Generic;

public interface IPathFindingGraph
{
    void GeneratePathTo(int x, int y);
    void SetDynamicObstacleNodes();
    List<Node> GetDynamicObstacleNodes();
    void GeneratePathfindingGrapgh();
}