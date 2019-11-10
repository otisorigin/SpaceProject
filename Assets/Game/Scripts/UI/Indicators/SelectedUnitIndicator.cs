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
            GameUtils.SetUnitScaleForObject(this, _unitManager.SelectedUnit);
            GameUtils.SetUnitPositionForObject(this, _unitManager.SelectedUnit);
//            var selectionCircleScale = _unitManager.SelectedUnit.GetScale();
//            transform.localScale = new Vector3(selectionCircleScale,
//                selectionCircleScale, 1.0f);
//            var position = _unitManager.SelectedUnit.transform.position;
//            transform.position = new Vector3(position.x, position.y, Constants.Coordinates.ZAxisUI);
        }
        else
        {
            _circleMeshRenderer.enabled = false;
        }
            
    }
}