using UnityEngine;
using Debug = UnityEngine.Debug;

public class ClickableTile : MonoBehaviour
{

    public int tileX;
    public int tileY;
    public TileMap map;
    
    // Start is called before the first frame update
    void OnMouseUp()
    {
        Debug.Log("Click x = " + tileX + ", y = " + tileY);
        //set this tile as end of the path for unit
        map.selectedUnit.GetComponent<Unit>().isPathSet = true;
        map.SetPathTo(tileX, tileY);
    }

    void OnMouseOver()
    {
        //show path to this tile
        if (!map.selectedUnit.GetComponent<Unit>().isPathSet)
        {
            map.GeneratePathTo(tileX, tileY);
        }
        
    }
}
