[System.Serializable]
public class TileType
{
    public string name;
    public ClickableTile tileVisualPrefab;

    public float movementCost = 1;
    public bool isWalkable = true;
}