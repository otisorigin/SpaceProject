using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

public class ClickableTile : MonoBehaviour
{
    //TODO исправить что в этом классе GameController не инжектится, а TileMap инжектится
    [Inject] private GameManager _manager;
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
            Map.manager.SelectedUnit.GetComponent<Unit>().isPathSet = true;
        }

        //map.SetPathTo(tileX, tileY);
    }

    void OnMouseOver()
    {
        //show path to this tile
        Debug.Log("x = " + tileX + ", y = " + tileY);
        if (Map.manager.CurrentState == GameManager.GameState.UnitMovement && Map.manager.SelectedUnit != null &&
            !Map.manager.SelectedUnit.GetComponent<Unit>().isPathSet)
        {
            Map.GeneratePathTo(tileX, tileY);
        }
    }
}