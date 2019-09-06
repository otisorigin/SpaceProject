using UnityEngine;
using Zenject;


public class PathIndicator : MonoBehaviour
{
    [Inject] private CursorManager cursorManager;
    [Inject] private GameManager _manager;

    // Start is called before the first frame update
    void Start()
    {
        var indicatorMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        indicatorMeshRenderer.material.color = Constants.Colors.DarkGreen;
        // mm = FindObjectOfType<MouseManager>();
        //transform.localScale = mm.selectedObject.gameObject.transform.localScale;
    }

    void Update()
    {
        SetIndicatorSize();
        OnCover();
        OnClick();
    }

    private void SetIndicatorSize()
    {
        if (_manager.SelectedUnit != null)
        {
            var selectedUnitLocalScale = _manager.SelectedUnit.transform.localScale;
            transform.localScale = new Vector3(selectedUnitLocalScale.y, selectedUnitLocalScale.y, selectedUnitLocalScale.z);  
        }
    }

    private void OnCover()
    {
        if (_manager.SelectedUnit != null && cursorManager.SelectedObject != null &&
            !cursorManager.SelectedObject.CompareTag("Unit") &&
            !cursorManager.SelectedObject.CompareTag("Barrier") && !_manager.SelectedUnit.isPathSet)
        {
            transform.position = cursorManager.SelectedObject.transform.position;
        }
    }

    private void OnClick()
    {
    }
}