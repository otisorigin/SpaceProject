using System;
using System.Linq;
using UnityEngine;
using Zenject;

public class SelectionManager : MonoBehaviour
{
    [Inject] private CursorManager _cursorManager;

//    [Inject] private TileMap _tileMap;

    [Inject] private GameManager _manager;

    private string _unitTag;
    private MeshRenderer _circleMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _unitTag = FindObjectOfType<Unit>().tag;
        _circleMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _circleMeshRenderer.enabled = false;
        //gameObject.SetActive(false);
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
        if (selectedObject != null && selectedObject.CompareTag(_unitTag))
        {
            
            var selectedUnit = selectedObject.GetComponent<Unit>();
            OnUnitHover(selectedUnit);
            OnUnitClick(selectedUnit);
        }
        else
        {
            _circleMeshRenderer.enabled = false;
        }
    }

    private void OnUnitHover(Unit unit)
    {
        if (!_manager.IsThisUnitSelected(unit))
        {
            _circleMeshRenderer.enabled = true;
            _circleMeshRenderer.material.color =
                _manager.IsUnitOfCurrentPlayer(unit) ? Color.cyan : Color.red;
            transform.position = unit.transform.position;
            var unitScale = unit.transform.localScale;
            var selectionCircleScale = UIUtils.GetBiggerScale(unitScale.x, unitScale.y);
            transform.localScale = new Vector3(selectionCircleScale,
                selectionCircleScale, unitScale.z);
        }
    }

    private void OnUnitClick(Unit unit)
    {
        if (_manager.IsUnitOfCurrentPlayer(unit) && Input.GetMouseButtonDown(0))
        {
            _manager.UnitSelect(unit);
            _circleMeshRenderer.enabled = false;
        }
    }

    
}
