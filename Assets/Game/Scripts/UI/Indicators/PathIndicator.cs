using System;
using UnityEngine;
using Zenject;


public class PathIndicator : MonoBehaviour
{
    [Inject] private CursorManager cursorManager;
    [Inject] private GameManager _manager;
    [Inject] private TileMap _map;

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
            transform.localScale =
                new Vector3(selectedUnitLocalScale.y, selectedUnitLocalScale.y, selectedUnitLocalScale.z);
        }
    }

    private void OnCover()
    {
        if (MovePathIndicatorCondition())
        {
            transform.position = cursorManager.SelectedObject.transform.position;
        }
    }

    private bool MovePathIndicatorCondition()
    {
        return _manager.SelectedUnit != null && cursorManager.SelectedObject != null &&
               !cursorManager.SelectedObject.CompareTag("Unit") &&
               !cursorManager.SelectedObject.CompareTag("Barrier") &&
               !_manager.SelectedUnit.isPathSet &&
               !_map.IsObstaclePresentOnTile(cursorManager.SelectedObject.transform.position);
    }

//    private bool CheckIndicatorCollision()
//    {
//        if (cursorManager.SelectedObject != null)
//        {
//            var objTranform = cursorManager.SelectedObject.transform;
//            if (objTranform.localScale.x > 1.0 && objTranform.localScale.y > 1.0)
//            {
//                //Debug.Log("--------Start-------------");
//                for (int i = (int) objTranform.position.y - 1; i < objTranform.position.y - 1 + objTranform.localScale.y; i++)
//                {
//                    for (int j = (int) objTranform.position.x - 1; j < objTranform.position.x - 1 + objTranform.localScale.x; j++)
//                    {
////                        Debug.Log("objTranform.position.x = " + (objTranform.position.x-1));
////                        Debug.Log("objTranform.position.y = " + (objTranform.position.y-1));
////                        Debug.Log("Y = " + i);
////                        Debug.Log("X = " + j);
//                        if (j == -1 || i == -1)
//                        {
//                            Debug.Log("false");
//                            return false;
//                        }
//                    }
//                }
//                //Debug.Log("--------END-------------");
////            for (int i = Convert.ToInt32(transform.position.y - 1f); i < transform.localScale.y; i++)
////            {
////                for (int j = Convert.ToInt32(transform.position.x - 1f); j < transform.localScale.x; j++)
////                {
////                    GameObject obj = _map.GetObjectFromCoord(j, i);
////                    if (obj != null &&
////                        obj.CompareTag("Unit") ||
////                        obj.CompareTag("Barrier") || _manager.SelectedUnit.isPathSet)
////                    {
////                        return false;
////                    }
////                }
////            }
//            }
//            return !cursorManager.SelectedObject.CompareTag("Unit") &&
//                   !cursorManager.SelectedObject.CompareTag("Barrier") && !_manager.SelectedUnit.isPathSet;
//        }
//        return false;
//    }

    private void OnClick()
    {
    }
}