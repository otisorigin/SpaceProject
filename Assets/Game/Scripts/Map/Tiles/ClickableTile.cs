using UnityEngine;

public class ClickableTile : MonoBehaviour
{
    public GameManager GameManager { get; set; }
    public UnitManager UnitManager { get; set; }
    public TileMap Map { get; set; }

    public int tileX;
    public int tileY;

    // Start is called before the first frame update
    void OnMouseUp()
    {
        if (GameManager.CurrentState == GameManager.GameState.UnitMovement)
        {
            var unit = UnitManager.GetSelectedUnitMovementSystem();
            unit.isPathSet = true;
            unit.IsMoving = true;
        }
    }

    void OnMouseOver()
    {
        //Debug.Log("Mouse over x: " + tileX + " y: " + tileY);
        if (Map != null && GameManager.CurrentState == GameManager.GameState.UnitMovement &&
            UnitManager.SelectedUnit != null &&
            !UnitManager.GetSelectedUnitMovementSystem().isPathSet && !UnitManager.GetSelectedUnitMovementSystem().IsMoving)
        {
            UnitManager.GeneratePathTo(tileX, tileY);
        }
    }
}