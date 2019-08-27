using UnityEngine;
using Zenject;

public class SelectionManager : MonoBehaviour
{
    [Inject] private CursorManager _cursorManager;

//    [Inject] private TileMap _tileMap;

    [Inject] private GameController _controller;

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
//        if (_controller.CurrentState == GameController.GameState.UnitSelection ||
//            _controller.CurrentState == GameController.GameState.UnitMovement)
//        {
            UnitSelection();
        //}
    }

    private void UnitSelection()
    {
        var selectedObject = _cursorManager.SelectedObject;
        if (selectedObject != null && selectedObject.CompareTag(_unit.tag))
        {
            //Renderer r;
            //r.bounds.size.x
            transform.position = selectedObject.transform.position;
            var selectedUnit = selectedObject.GetComponent<Unit>();
            if (_controller.IsUnitOfCurrentPlayer(selectedUnit) && Input.GetMouseButtonDown(0))
            {
                _controller.UnitSelect(selectedUnit);
            }
        }
    }
}