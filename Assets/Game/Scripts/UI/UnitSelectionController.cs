public class UnitSelectionController
{
    private Unit selectedUnit;

    public void unitSelect(Unit unit)
    {
        selectedUnit.isSelected = false;
        selectedUnit = unit;
        selectedUnit.isSelected = true;
    }
}