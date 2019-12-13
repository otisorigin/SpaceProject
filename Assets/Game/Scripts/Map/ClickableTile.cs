using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

public class ClickableTile : MonoBehaviour
{
    //TODO исправить что в этом классе GameController не инжектится, а TileMap инжектится
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;
    [Inject] public TileMap Map { get; set; }

    public int tileX;
    public int tileY;


    // Start is called before the first frame update
    void OnMouseUp()
    {
        Debug.Log("Click x = " + tileX + ", y = " + tileY);
        //set this tile as end of the path for unit
        if (Map.manager.CurrentState == GameManager.GameState.UnitMovement)
        {
            var unit = Map._unitManager.SelectedUnit.GetComponent<Unit>();
            unit.isPathSet = true;
            unit.IsMoving = true;
        }
    }

    void OnMouseOver()
    {
        //TODO сделать не по clicable tile (потому что для 2х2 мы нажимаем не на тайлы а между ними)
        //show path to this tile
        //Debug.Log("x = " + tileX + ", y = " + tileY);
        if (Map.manager.CurrentState == GameManager.GameState.UnitMovement && Map._unitManager.SelectedUnit != null &&
            !Map._unitManager.SelectedUnit.GetComponent<Unit>().isPathSet)
        {
            Map._unitManager.GeneratePathTo(tileX, tileY);
        }
    }
}