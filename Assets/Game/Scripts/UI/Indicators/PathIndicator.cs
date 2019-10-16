using UnityEngine;
using Zenject;


public class PathIndicator : MonoBehaviour
{
    [Inject] private CursorManager cursorManager;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;
    [Inject] private TileMap _map;

    void Start()
    {
        var indicatorMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        indicatorMeshRenderer.material.color = Constants.Colors.DarkGreen;
    }

    void Update()
    {
        if (_unitManager.SelectedUnit != null)
        {
            SetIndicatorSize();
            SetIndicatorPosition();
        }
    }

    private void SetIndicatorSize()
    {
        var selectedUnitLocalScale = _unitManager.SelectedUnit.transform.localScale;
        transform.localScale =
            new Vector3(selectedUnitLocalScale.y, selectedUnitLocalScale.y, selectedUnitLocalScale.z);
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
}