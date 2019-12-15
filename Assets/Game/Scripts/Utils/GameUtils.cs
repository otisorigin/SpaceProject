using UnityEngine;

public class GameUtils
{

    public static void SetUnitScaleForObject(MonoBehaviour obj, Unit unit, float factor = 0.0f)
    {
        var scale = unit.GetScale()+factor;
        obj.transform.localScale = new Vector3(scale,scale, 1.0f);
    }

    public static void SetUnitPositionForObject(MonoBehaviour obj, Unit unit)
    {
        var position = unit.transform.position;
        obj.transform.position = new Vector3(position.x, position.y, Constants.Coordinates.ZAxisUI);
    }
    
}