using System.Collections.Generic;

public interface IPathFindingGraph
{
    void GeneratePathTo(float x, float y);
    void SetDynamicObstacleNodes();
    List<Node> GetDynamicObstacleNodes();
    void GeneratePathFindingGraph();
}