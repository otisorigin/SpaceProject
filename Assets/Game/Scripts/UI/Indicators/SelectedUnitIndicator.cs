using UnityEngine;
using Zenject;

public class SelectedUnitIndicator : MonoBehaviour
{
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;
    private MeshRenderer _circleMeshRenderer;

    private void Start()
    {
        _circleMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _circleMeshRenderer.material.color = Constants.Colors.DarkGreen;
        _circleMeshRenderer.enabled = false;
    }

    private void Update()
    {
        if (_manager.CurrentState == GameManager.GameState.UnitMovement ||
            _manager.CurrentState == GameManager.GameState.UnitAttack)
        {
            _circleMeshRenderer.enabled = true;
            var selectedUnitTransform = _unitManager.SelectedUnit.transform;
            var unitScale = selectedUnitTransform.localScale;
            var selectionCircleScale = UIUtils.GetBiggerScale(unitScale.x, unitScale.y);
            transform.localScale = new Vector3(selectionCircleScale,
                selectionCircleScale, unitScale.z);
            transform.position = selectedUnitTransform.position;
        }
        else
        {
            _circleMeshRenderer.enabled = false;
        }
            
    }
}