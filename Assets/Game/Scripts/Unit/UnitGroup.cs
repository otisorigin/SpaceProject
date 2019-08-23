using UnityEngine;

public class UnitGroup : MonoBehaviour, IUnitGroup
{
    public Unit SelectedUnit { get; private set; }
    public Unit[] firstUserUnitList;
    public Unit[] secondUserUnitsList;

    void Start()
    {
        SelectedUnit = firstUserUnitList[0].GetComponent<Unit>();
    }

    public void ResetPath()
    {
        SelectedUnit.currentPath = null;
        SelectedUnit.isPathSet = false;
    }

    public void NextTurn()
    {
        
    }

    public void UnitSelect(Unit selectedUnit)
    {
        SelectedUnit = selectedUnit;
    }
}