using UnityEngine;

public class GameUtils
{

    public static void SetUnitScaleForObject(MonoBehaviour obj, Unit unit)
    {
        var unitScale = unit.GetScale();
        obj.transform.localScale = new Vector3(unitScale,unitScale, 1.0f);
    }

    public static void SetUnitPositionForObject(MonoBehaviour obj, Unit unit)
    {
        var position = unit.transform.position;
        obj.transform.position = new Vector3(position.x, position.y, Constants.Coordinates.ZAxisUI);
    }
    
}