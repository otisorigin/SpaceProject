using UnityEngine;
using Zenject;

public class SelectionManager : MonoBehaviour
{
    [Inject] private CursorManager _cursorManager;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;

    private string _unitTag;
    private MeshRenderer _circleMeshRenderer;

    void Start()
    {
        _unitTag = FindObjectOfType<Unit>().tag;
        _circleMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _circleMeshRenderer.enabled = false;
    }

    void Update()
    {
        UnitSelection();
    }

    private void UnitSelection()
    {
        var selectedObject = _cursorManager.SelectedObject;
        if ((_unitManager.SelectedUnit == null ||
             !_unitManager.GetSelectedUnitMovementSystem().IsMoving) && selectedObject != null &&
            selectedObject.CompareTag(_unitTag))
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
        if (!_unitManager.IsThisUnitSelected(unit))
        {
            _circleMeshRenderer.enabled = true;
            if (_manager.IsUnitOfCurrentPlayer(unit))
            {
                if (unit.GetComponentInChildren<MovementSystem>().RemainingMovement == 0)
                {
                    _circleMeshRenderer.material.color = Constants.Colors.DarkYellow;
                }
                else
                {
                    _circleMeshRenderer.material.color = Color.cyan;
                }
            }
            else
            {
                _circleMeshRenderer.material.color = Color.red;
            }

            GameUtils.SetUnitScaleForObject(this, unit, 0.2f);
            GameUtils.SetUnitPositionForObject(this, unit);
        }
    }

    private void OnUnitClick(Unit unit)
    {
        if (_manager.IsUnitOfCurrentPlayer(unit) && !_unitManager.IsThisUnitSelected(unit) &&
            unit.GetComponentInChildren<MovementSystem>().RemainingMovement != 0 &&
            Input.GetMouseButtonDown(0))
        {
            _unitManager.UnitSelect(unit);
            _circleMeshRenderer.enabled = false;
        }
    }
}