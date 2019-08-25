using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

public class ClickableTile : MonoBehaviour
{
    //TODO исправить что unitGroup не инжектится, хотя map и unitGroup внутри него работают ок
    [Inject] 
    private UnitGroup _unitGroup;
    [Inject]
    public TileMap Map { get; set; }

    public int tileX;
    public int tileY;
    
    
    // Start is called before the first frame update
    void OnMouseUp()
    {
        Debug.Log("Click x = " + tileX + ", y = " + tileY);
        //set this tile as end of the path for unit
        Map._unitGroup.SelectedUnit.GetComponent<Unit>().isPathSet = true;
        //map.SetPathTo(tileX, tileY);
    }

    void OnMouseOver()
    {
        //show path to this tile
        if (Map._unitGroup.SelectedUnit != null && !Map._unitGroup.SelectedUnit.GetComponent<Unit>().isPathSet)
        {
            Map.GeneratePathTo(tileX, tileY);
        }
        
    }
}
