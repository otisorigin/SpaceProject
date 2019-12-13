using UnityEngine;
using Zenject;


public class PathIndicator : MonoBehaviour
{
    [Inject] private CursorManager cursorManager;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;
    [Inject] private TileMap _map;
    
    private void Awake()
    {
        _unitManager.OnUnitSelect += HandleUnitSelect;
    }

    void Start()
    {
        var indicatorMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        indicatorMeshRenderer.material.color = Constants.Colors.DarkGreen;
    }

    void Update()
    {
        if (_unitManager.SelectedUnit != null)
        {
            GameUtils.SetUnitScaleForObject(this, _unitManager.SelectedUnit);
            SetIndicatorPosition();
        }
    }

    private void SetIndicatorPosition()
    {
        var currentPath = _unitManager.SelectedUnit.CurrentPath;
        if (currentPath != null)
        {
            var node = currentPath[currentPath.Count - 1];
            transform.position = new Vector3(node.x, node.y, _unitManager.SelectedUnit.transform.position.z);
        }
    }
    
    private void HandleUnitSelect(Unit selectedUnit)
    {
        transform.position = new Vector3(selectedUnit.tileX, selectedUnit.tileY, selectedUnit.transform.position.z);
    }
}