using System.Linq;
using UnityEngine;

public class UnitGroup : MonoBehaviour
{
    public Unit SelectedUnit { get; set; }
    public string CurrentUser { get; set; }
    public Unit[] firstUserUnitList;
    public Unit[] secondUserUnitsList;

    public void ResetPath()
    {
        if (SelectedUnit != null)
        {
            SelectedUnit.CurrentPath = null;
            SelectedUnit.isPathSet = false;
        }
    }

    public void NextTurn()
    {
        firstUserUnitList.ToList().ForEach(unit => unit.NextTurn());
    }

    public void UnitSelect(Unit selectedUnit)
    {
        SelectedUnit = selectedUnit;
    }
}