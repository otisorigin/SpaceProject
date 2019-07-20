using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Unit;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    private MouseManager mm;

    private Unit _unit;
    // Start is called before the first frame update
    void Start()
    {
        _unit = GameObject.FindObjectOfType<Unit>();
        mm = GameObject.FindObjectOfType<MouseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mm.selectedObject != null && mm.selectedObject.CompareTag(_unit.tag))
        {
            this.transform.position = mm.selectedObject.transform.position;
            if (Input.GetMouseButtonDown(0))
            {
                UnitSelection(mm.selectedObject.GetComponent<Unit>());
            }
        }
   
    }

    private void UnitSelection(Unit selectedUnit)
    {
        mm.map.UnitSelect(selectedUnit);
    }
}
