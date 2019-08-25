using UnityEngine;
using Zenject;

public class SelectionIndicator : MonoBehaviour
{
    [Inject] private MouseManager _mouseManager;

//    [Inject] private TileMap _tileMap;
    
    [Inject] private UnitGroup _unitGroup;

    private Unit _unit;
    // Start is called before the first frame update
    void Start()
    {
        _unit = FindObjectOfType<Unit>();
        //_mouseManager = FindObjectOfType<MouseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        var selectedObject = _mouseManager.SelectedObject;
        if (selectedObject != null &&selectedObject.CompareTag(_unit.tag))
        {
            transform.position = selectedObject.transform.position;
            if (Input.GetMouseButtonDown(0))
            {
                UnitSelection(selectedObject.GetComponent<Unit>());
            }
        }
    }

    private void UnitSelection(Unit selectedUnit)
    {
        _unitGroup.UnitSelect(selectedUnit);
    }
}
